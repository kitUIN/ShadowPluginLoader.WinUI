using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using DryIoc;
using ShadowPluginLoader.WinUI.Services;

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
                        beforeSort.Add(
                            await GetPluginInstaller(uri)
                                .LoadSortPluginData<TMeta>(uri, TempFolder))
                    )
                );
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
            beforeSort.Add(await GetPluginInstaller(sortPluginData.InstallerId)
                .InstallAsync(sortPluginData, TempFolder, PluginFolder));
        })));
        await Task.WhenAll(beforeSortTasks);
        foreach (var sortPluginData in beforeSort)
        {
            try
            {
                var mainPluginType = await GetPluginInstaller(sortPluginData.InstallerId).GetMainPluginType(sortPluginData);
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
    /// 预加载插件DLL（不实例化）
    /// </summary>
    /// <returns>预加载的插件数据</returns>
    public async Task<List<PreloadedPluginData<TMeta>>> PreloadAsync()
    {
        if (!IsCheckUpgradeAndRemove)
            throw new Exception("You need to try CheckUpgradeAndRemoveAsync before PreloadAsync");

        var preloadedPlugins = new List<PreloadedPluginData<TMeta>>();
        var beforeSort = new ConcurrentBag<SortPluginData<TMeta>>();
        var beforeSortTasks = new List<Task>();

        // 第一阶段：扫描和安装插件文件
        while (ScanQueue.Count > 0)
        {
            try
            {
                var uri = ScanQueue.Dequeue();
                beforeSortTasks.Add(Task.Run(async () =>
                        beforeSort.Add(
                            await GetPluginInstaller(uri)
                                .LoadSortPluginData<TMeta>(uri, TempFolder))
                    )
                );
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

        // 依赖关系排序
        var afterSortResult = DependencyChecker.DetermineLoadOrder(beforeSort.ToList());
        beforeSortTasks.Clear();
        beforeSort.Clear();

        // 并行安装插件文件
        beforeSortTasks.AddRange(afterSortResult.Result.Select(sortPluginData => Task.Run(async () =>
        {
            beforeSort.Add(await GetPluginInstaller(sortPluginData.InstallerId)
                .InstallAsync(sortPluginData, TempFolder, PluginFolder));
        })));

        await Task.WhenAll(beforeSortTasks);

        // 第二阶段：预加载DLL（不实例化）
        var preloadTasks = beforeSort.Select(async sortPluginData =>
        {
            try
            {
                var mainPluginType = await GetPluginInstaller(sortPluginData.InstallerId)
                    .GetMainPluginType(sortPluginData);
                
                var assembly = mainPluginType.Assembly;
                
                var preloadedData = new PreloadedPluginData<TMeta>
                {
                    MetaData = sortPluginData.MetaData,
                    MainPluginType = mainPluginType,
                    Assembly = assembly,
                    InstallerId = sortPluginData.InstallerId,
                    Path = sortPluginData.Path,
                    Dependencies = sortPluginData.Dependencies.ToList(),
                    Priority = sortPluginData.Priority,
                    Version = sortPluginData.Version
                };

                _preloadedPlugins[sortPluginData.MetaData.Id] = preloadedData;
                preloadedPlugins.Add(preloadedData);

                Logger.Information("{Pre}{ID}: DLL Preloaded Successfully", 
                    LoggerPrefix, sortPluginData.MetaData.Id);
            }
            catch (Exception e)
            {
                Logger.Warning("Preload Error:{Ex}", e);
            }
        });

        await Task.WhenAll(preloadTasks);

        return preloadedPlugins;
    }

    /// <summary>
    /// 实例化预加载的插件
    /// </summary>
    /// <param name="pluginIds">要实例化的插件ID列表，为空则实例化所有</param>
    /// <returns>实例化的插件数量</returns>
    public async Task<int> InstantiatePluginsAsync(IEnumerable<string>? pluginIds = null)
    {
        var targetIds = pluginIds?.ToList() ?? _preloadedPlugins.Keys.ToList();
        var instantiatedCount = 0;

        foreach (var pluginId in targetIds)
        {
            if (!_preloadedPlugins.TryGetValue(pluginId, out var preloadedData))
            {
                Logger.Warning("{Pre}Plugin {ID} not found in preloaded data", LoggerPrefix, pluginId);
                continue;
            }

            if (preloadedData.IsInstantiated)
            {
                Logger.Information("{Pre}Plugin {ID} already instantiated", LoggerPrefix, pluginId);
                continue;
            }

            try
            {
                await InstantiatePluginAsync(preloadedData);
                instantiatedCount++;
            }
            catch (Exception e)
            {
                Logger.Warning("{Pre}Failed to instantiate plugin {ID}: {Ex}", LoggerPrefix, pluginId, e);
            }
        }

        return instantiatedCount;
    }

    /// <summary>
    /// 实例化单个插件
    /// </summary>
    /// <param name="preloadedData">预加载的插件数据</param>
    private async Task InstantiatePluginAsync(PreloadedPluginData<TMeta> preloadedData)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            BeforeLoadPlugin(preloadedData.MainPluginType, preloadedData.MetaData);
            var instance = LoadMainPlugin(preloadedData.MainPluginType, preloadedData.MetaData);
            AfterLoadPlugin(preloadedData.MainPluginType, instance, preloadedData.MetaData);
            
            _plugins[preloadedData.MetaData.Id] = instance;
            preloadedData.Instance = instance;
            preloadedData.IsInstantiated = true;
            preloadedData.InstantiatedAt = DateTime.Now;

            var enabled = PluginSettingsHelper.GetPluginIsEnabled(preloadedData.MetaData.Id);
            instance.Loaded();
            PluginEventService.InvokePluginLoaded(this, new PluginEventArgs(preloadedData.MetaData.Id, PluginStatus.Loaded));
            
            stopwatch.Stop();
            Logger.Information("{Pre}{ID}({isEnabled}): Instantiated Successfully! Used: {mi} ms",
                LoggerPrefix, preloadedData.MetaData.Id, enabled, stopwatch.ElapsedMilliseconds);
            
            DependencyChecker.LoadedPlugins.Add(preloadedData.MetaData.DllName, new Version(preloadedData.MetaData.Version));
            
            if (enabled)
            {
                instance.IsEnabled = enabled;
            }
        }
        catch (Exception e)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            Logger.Warning("{Pre}Plugin Instantiation Failed! Used: {mi} ms, Error: {Ex}",
                LoggerPrefix, stopwatch.ElapsedMilliseconds, e);
            throw;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }
    }

    /// <summary>
    /// 获取预加载的插件数据
    /// </summary>
    /// <returns>预加载的插件数据列表</returns>
    public IList<PreloadedPluginData<TMeta>> GetPreloadedPlugins()
    {
        return _preloadedPlugins.Values.ToList();
    }

    /// <summary>
    /// 获取指定插件的预加载数据
    /// </summary>
    /// <param name="id">插件ID</param>
    /// <returns>预加载的插件数据或null</returns>
    public PreloadedPluginData<TMeta>? GetPreloadedPlugin(string id)
    {
        return _preloadedPlugins.GetValueOrDefault(id);
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