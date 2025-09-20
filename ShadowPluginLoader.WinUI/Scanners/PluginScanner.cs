using CustomExtensions.WinUI;
using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using ShadowPluginLoader.WinUI.Helpers;

namespace ShadowPluginLoader.WinUI.Scanners;

/// <inheritdoc />
public class PluginScanner<TAPlugin, TMeta> : IPluginScanner<TAPlugin, TMeta>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; } = Log.ForContext<PluginScanner<TAPlugin, TMeta>>();

    /// <summary>
    /// checked for updates and removed plugins
    /// </summary>
    protected bool IsCheckUpgradeAndRemove = false;

    /// <summary>
    /// DependencyChecker
    /// </summary>
    protected readonly IDependencyChecker<TMeta> DependencyChecker;

    /// <summary>
    /// Plugin Scanner
    /// </summary>
    /// <param name="dependencyChecker">Dependency Checker</param>
    public PluginScanner(IDependencyChecker<TMeta> dependencyChecker)
    {
        DependencyChecker = dependencyChecker;
    }

    /// <summary>
    /// Used to store Tasks that convert JSON files to TMeta
    /// </summary>
    protected readonly ConcurrentBag<Task<SortPluginData<TMeta>>> ScanTaskList = [];

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginScanner<TAPlugin, TMeta> Scan(Type? type)
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
    public IPluginScanner<TAPlugin, TMeta> Scan<TPlugin>()
    {
        Scan(typeof(TPlugin));
        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginScanner<TAPlugin, TMeta> Scan(params Type?[] types)
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
    public IPluginScanner<TAPlugin, TMeta> Scan(IEnumerable<Type?> types)
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
    public IPluginScanner<TAPlugin, TMeta> Scan(Package package)
    {
        return Scan(new DirectoryInfo(package.InstalledPath));
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginScanner<TAPlugin, TMeta> Scan(DirectoryInfo dir)
    {
        foreach (var assetDir in dir.EnumerateDirectories("Assets", SearchOption.AllDirectories))
        {
            var pluginPath = Path.Combine(assetDir.FullName, "plugin.json");
            if (File.Exists(pluginPath)) Scan(new Uri(pluginPath));
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public IPluginScanner<TAPlugin, TMeta> Scan(FileInfo pluginJson)
    {
        if (File.Exists(pluginJson.FullName)) Scan(new Uri(pluginJson.FullName));
        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginScanException"></exception>
    public IPluginScanner<TAPlugin, TMeta> Scan(Uri uri)
    {
        if (!uri.IsFile && Directory.Exists(uri.LocalPath))
        {
            Scan(new DirectoryInfo(uri.LocalPath));
        }
        else
        {
            ScanTaskList.Add(Task.Run(async () =>
            {
                var content = await File.ReadAllTextAsync(uri.LocalPath);
                var meta = MetaDataHelper.ToMeta<TMeta>(content) ??
                           throw new PluginScanException($"Failed to deserialize plugin metadata from {uri}");
                return new SortPluginData<TMeta>(meta, uri);
            }));
        }

        return this;
    }

    /// <inheritdoc />
    public void ScanClear()
    {
        ScanTaskList.Clear();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> Finish()
    {
        if (!IsCheckUpgradeAndRemove)
            throw new PluginScanException("You need to try CheckUpgradeAndRemoveAsync before LoadAsync");
        var scanTaskArray = ScanTaskList.ToArray();
        ScanClear();
        List<SortPluginData<TMeta>> beforeSorts = [.. await Task.WhenAll(scanTaskArray)];
        var sortResult = DependencyChecker.DetermineLoadOrder(beforeSorts.ToList());
        await Task.WhenAll(sortResult.Result.Select(GetMainPluginType).ToArray());
        sortResult.Result.ForEach(t => DependencyChecker.LoadedMetas[t.Id] = t.MetaData);
        return sortResult.Result.Select(sortPlugin => sortPlugin.Id);
    }

    /// <summary>
    /// Pre Load
    /// </summary>
    /// <param name="sortPluginData"></param>
    /// <returns></returns>
    /// <exception cref="PluginScanException"></exception>
    private async Task<Tuple<string, Type>> GetMainPluginType(SortPluginData<TMeta> sortPluginData)
    {
        var dllFilePath =
            Path.GetFullPath(Path.GetDirectoryName(sortPluginData.Path!)! + "/../"+ sortPluginData.MetaData.DllName);
        if (!File.Exists(dllFilePath)) throw new PluginScanException($"Not Found {dllFilePath}");

        var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(dllFilePath);
        var assembly = asm.ForeignAssembly;

        var entryPoints = sortPluginData.MetaData.EntryPoints;
        if (entryPoints.FirstOrDefault(x => x.Name == "MainPlugin") is not { } pluginEntryPoint)
        {
            throw new PluginScanException($"{sortPluginData.Id} MainPlugin(EntryPoint) Not Found");
        }

        var mainType = assembly.GetType(pluginEntryPoint.Type);
        if (mainType == null)
        {
            throw new PluginScanException($"{sortPluginData.Id} MainPlugin(Type) Not Found");
        }

        DiFactory.Services.Register(typeof(TAPlugin), mainType, reuse: Reuse.Singleton,
            serviceKey: sortPluginData.Id, made: Parameters.Of.Type(_ => sortPluginData.MetaData));
        return new Tuple<string, Type>(sortPluginData.Id, mainType);
    }


    /// <summary>
    /// Check Any Plugin Plan To Upgrade
    /// </summary>
    protected async Task CheckUpgrade()
    {
        // var settings = PluginSettingsHelper.GetPluginUpgradePaths();
        // var targetPaths = PluginSettingsHelper.GetPluginUpgradeTargetPaths();
        // foreach (var setting in settings)
        // {
        //     var uri = new Uri((string)setting.Value);
        //     await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(setting.Key))
        //         .UpgradeAsync(setting.Key, uri, TempFolder, (string)targetPaths[setting.Key]);
        // }
    }

    /// <summary>
    /// Check Any Plugin Plan To Remove
    /// </summary>
    protected async Task CheckRemove()
    {
        // var settings = PluginSettingsHelper.GetPluginRemovePaths();
        // foreach (var setting in settings)
        // {
        //     var path = (string)setting.Value;
        //     await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(setting.Key))
        //         .RemoveAsync(setting.Key, TempFolder, path);
        //     PluginEventService.InvokePluginRemoved(this,
        //         new Args.PluginEventArgs(setting.Key, Enums.PluginStatus.Removed));
        // }
    }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await CheckRemove();
        await CheckUpgrade();
        IsCheckUpgradeAndRemove = true;
    }
}