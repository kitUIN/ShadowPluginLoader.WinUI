using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// <inheritdoc />
/// </summary>
public abstract partial class AbstractPluginLoader<TMeta, TAPlugin> : IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    #region Scan

    /// <summary>
    /// ScanQueue
    /// </summary>
    protected readonly Queue<Uri> ScanQueue = new();

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(Type? type)
    {
        if (type is null) return this;
        var dir = type.Assembly.Location[..^".dll".Length];
        var metaPath = Path.Combine(dir, "Assets", "plugin.json");
        Scan(new Uri(metaPath));
        return this;
    }


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan<TPlugin>()
    {
        Scan(typeof(TPlugin));
        return this;
    }


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Scan(type);
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(Package package)
    {
        return Scan(new DirectoryInfo(package.InstalledPath));
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(DirectoryInfo dir)
    {
        foreach (var pluginJson in dir.GetDirectories("Assets", SearchOption.AllDirectories)
                     .Select(assetDir => new FileInfo(Path.Combine(assetDir.FullName, "plugin.json")))
                     .Where(file => file.Exists))
        {
            Scan(pluginJson);
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(FileInfo pluginJson)
    {
        ScanQueue.Enqueue(new Uri(pluginJson.FullName));
        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(Uri uri)
    {
        if (uri.IsFile && Directory.Exists(uri.LocalPath))
        {
            Scan(new DirectoryInfo(uri.LocalPath));
        }
        else
        {
            ScanQueue.Enqueue(uri);
        }

        return this;
    }

    /// <inheritdoc />
    public void ScanClear()
    {
        ScanQueue.Clear();
    }

    #endregion

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task<List<SortPluginData<TMeta>>> LoadAsync()
    {
        if (!IsCheckUpgradeAndRemove)
            throw new Exception("You need to try CheckUpgradeAndRemoveAsync before LoadAsync");
        ConcurrentBag<SortPluginData<TMeta>> beforeSort = [];
        List<Task> beforeSortTasks = [];
        while (ScanQueue.Count > 0)
        {
            try
            {
                var uri = ScanQueue.Dequeue();
                beforeSortTasks.Add(Task.Run(async () =>
                    {
                        beforeSort.Add(await MetaDataChecker.LoadSortPluginData(uri, TempFolder));
                    }
                ));
            }
            catch (PluginImportException e)
            {
                Logger.Warning("PreLoad Error:{Ex}", e);
            }
            catch (InvalidOperationException)
            {
                break;
            }
        }

        await Task.WhenAll(beforeSortTasks);
        // 排序
        var afterSortResult = DependencyChecker.DetermineLoadOrder(beforeSort.ToList());
        beforeSortTasks.Clear();
        beforeSort.Clear();
        beforeSortTasks.AddRange(afterSortResult.Result.Select(sortPluginData => Task.Run(async () =>
        {
            beforeSort.Add(await GetPluginInstaller(sortPluginData.Link)
                .InstallAsync(sortPluginData, TempFolder, PluginFolder));
        })));
        await Task.WhenAll(beforeSortTasks);
        foreach (var sortPluginData in beforeSort)
        {
            try
            {
                var mainPluginType = await MetaDataChecker.GetMainPluginType(sortPluginData);
                LoadPlugin(mainPluginType, sortPluginData.MetaData);
            }
            catch (Exception e)
            {
                Logger.Warning("LoadAsync Error:{Ex}", e);
            }
        }

        return afterSortResult.NeedUpgradeResult;
    }

    /// <summary>
    /// Check Any Plugin Plan To Upgrade
    /// </summary>
    protected async Task CheckUpgrade()
    {
        var settings = PluginSettingsHelper.GetPluginUpgradePaths();
        var targetPaths = PluginSettingsHelper.GetPluginUpgradeTargetPaths();
        foreach (var setting in settings)
        {
            var uri = new Uri((string)setting.Value);
            await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(setting.Key))
                .UpgradeAsync(setting.Key, uri, TempFolder, (string)targetPaths[setting.Key]);
        }
    }

    /// <summary>
    /// Check Any Plugin Plan To Remove
    /// </summary>
    protected async Task CheckRemove()
    {
        var settings = PluginSettingsHelper.GetPluginRemovePaths();
        foreach (var setting in settings)
        {
            var path = (string)setting.Value;
            await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(setting.Key))
                .RemoveAsync(setting.Key, TempFolder, path);
            PluginEventService.InvokePluginRemoved(this,
                new Args.PluginEventArgs(setting.Key, Enums.PluginStatus.Removed));
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool? IsEnabled(string id)
    {
        if (_plugins.TryGetValue(id, out var plugin))
        {
            return plugin.IsEnabled;
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IList<TAPlugin> GetPlugins()
    {
        return _plugins.Values.ToList();
    }


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public TAPlugin? GetPlugin(string id)
    {
        return _plugins.GetValueOrDefault(id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IList<TAPlugin> GetEnabledPlugins()
    {
        return _plugins
            .Where(plugin => plugin.Value.IsEnabled)
            .Select(plugin => plugin.Value)
            .ToList();
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public TAPlugin? GetEnabledPlugin(string id)
    {
        if (GetPlugin(id) is { IsEnabled: true } plugin) return plugin;
        return null;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void EnablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = true;
        Logger.Information("{Pre}{Id}: Enabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void DisablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = false;
        Logger.Information("{Pre}{Id}: Disabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task RemovePlugin(string id)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginRemoveException($"{id} Plugin Not Found");
        var path = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (path == null) throw new PluginRemoveException($"{id} Plugin Path Not Found");
        await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(id))
            .PreRemoveAsync(plugin, TempFolder, path);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="meta"></param>
    /// <param name="pluginMetaData"></param>
    /// <exception cref="PluginUpgradeException"></exception>
    protected void CheckNewVersionMeta(TMeta meta, TMeta pluginMetaData)
    {
        if (new Version(meta.Version) <= new Version(pluginMetaData.Version))
        {
            throw new PluginUpgradeException($"The upgraded version ({meta.Version}) " +
                                             $"should be larger than the current " +
                                             $"version ({pluginMetaData.Version})");
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task UpgradePlugin(string id, Uri uri)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginUpgradeException($"{id} Plugin not found");
        await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(id))
            .PreUpgradeAsync(plugin, uri, TempFolder, PluginFolder);
    }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await CheckRemove();
        await CheckUpgrade();
        IsCheckUpgradeAndRemove = true;
    }
}