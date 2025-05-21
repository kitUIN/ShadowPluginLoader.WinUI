using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;

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
    protected readonly Queue<ScanTarget> ScanQueue = new();

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginLoader<TMeta, TAPlugin> Scan(Type? type)
    {
        if (type is null) return this;
        var dir = type.Assembly.Location[..^".dll".Length];
        var metaPath = Path.Combine(dir, "Assets", "plugin.json");
        Scan(new FileInfo(metaPath));
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
        ScanQueue.Enqueue(pluginJson.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? new ScanTarget(ScanType.FileInfo, pluginJson)
            : new ScanTarget(ScanType.Uri, new Uri(pluginJson.FullName)));
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
            ScanQueue.Enqueue(new ScanTarget(ScanType.Uri, uri));
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
    public async Task<List<string>> Load()
    {
        if (!IsCheckUpgradeAndRemove)
            throw new Exception("You need to try CheckUpgradeAndRemoveAsync before Load");
        List<SortPluginData<TMeta>> beforeSort = [];
        List<Task> beforeSortTasks = [];
        while (ScanQueue.Count > 0)
        {
            try
            {
                var scanTarget = ScanQueue.Dequeue();
                beforeSortTasks.Add(Task.Run(async () =>
                    {
                        FileInfo? target = null;
                        if (scanTarget.Type == ScanType.Uri)
                        {
                            var uri = (Uri)scanTarget.Target;
                            var installer = GetPluginInstaller(uri);
                            if (installer is null)
                            {
                                Logger.Warning("{URI} Not Found Installer", uri);
                                return;
                            }

                            target = await installer.ScanAsync(uri, TempFolder, PluginFolder);
                        }
                        else
                        {
                            target = (FileInfo)scanTarget.Target;
                        }

                        beforeSort.Add(new SortPluginData<TMeta>(await MetaDataChecker.LoadMetaData(target), target));
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
        var afterSortResult = DependencyChecker.DetermineLoadOrder(beforeSort);
        foreach (var sortPluginData in afterSortResult.Result)
        {
            try
            {
                var mainPluginType = await MetaDataChecker.GetMainPluginType(sortPluginData.MetaData);
                LoadPlugin(mainPluginType, sortPluginData.MetaData);
            }
            catch (Exception e)
            {
                Logger.Warning("Load Error:{Ex}", e);
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
            var installer = GetPluginInstaller(uri);
            if (installer is null) continue;
            await installer.UpgradeAsync(setting.Key, uri, TempFolder, (string)targetPaths[setting.Key]);
            PluginSettingsHelper.DeleteUpgradePluginPath(setting.Key);
        }
    }

    /// <summary>
    /// Check Any Plugin Plan To Remove
    /// </summary>
    protected void CheckRemove()
    {
        var settings = PluginSettingsHelper.GetPluginRemovePaths();
        foreach (var setting in settings)
        {
            var path = (string)setting.Value;
            if (Directory.Exists(path)) Directory.Delete(path, true);
            PluginSettingsHelper.DeleteRemovePluginPath(setting.Key);
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
    public void RemovePlugin(string id)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginRemoveException($"{id} Plugin Not Found");
        var path = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (path == null) throw new PluginRemoveException($"{id} Plugin Path Not Found");
        plugin.PlanRemove = true;
        PluginSettingsHelper.SetPluginPlanRemove(id, path);
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
    public async Task UpgradePlugin(string id, string newVersionZip)
    {
        newVersionZip = await DownloadHelper.DownloadFileAsync(TempFolder, newVersionZip, Logger);
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginUpgradeException($"{id} Plugin not found");
        plugin.PlanUpgrade = true;
        var path = plugin.GetType().Assembly.Location;
        PluginSettingsHelper.SetPluginUpgradePath(id, newVersionZip, path);
    }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        CheckRemove();
        await CheckUpgrade();
        IsCheckUpgradeAndRemove = true;
    }
}