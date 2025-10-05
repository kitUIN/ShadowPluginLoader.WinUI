using CustomExtensions.WinUI;
using DryIoc;
using NuGet.Versioning;
using Serilog;
using ShadowObservableConfig;
using ShadowObservableConfig.Attributes;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ShadowPluginLoader.WinUI.Scanners;

/// <inheritdoc />
public class PluginScanner<TAPlugin, TMeta> : IPluginScanner<TAPlugin, TMeta>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    private readonly SemaphoreSlim _finishLock = new(1, 1);
    private readonly ConcurrentDictionary<Guid, PluginScanSession<TAPlugin, TMeta>> _activeSessions = new();


    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; } = Log.ForContext<PluginScanner<TAPlugin, TMeta>>();


    /// <summary>
    /// UpgradeChecker
    /// </summary>
    protected readonly IUpgradeChecker UpgradeChecker;

    /// <summary>
    /// RemoveChecker
    /// </summary>
    protected readonly IRemoveChecker RemoveChecker;

    /// <summary>
    /// DependencyChecker
    /// </summary>
    protected readonly IDependencyChecker<TMeta> DependencyChecker;


    /// <summary>
    /// Plugin Scanner
    /// </summary>
    /// <param name="dependencyChecker">Dependency Checker</param>
    /// <param name="upgradeChecker"></param>
    /// <param name="removeChecker"></param>
    public PluginScanner(IDependencyChecker<TMeta> dependencyChecker,
        IUpgradeChecker upgradeChecker, IRemoveChecker removeChecker)
    {
        DependencyChecker = dependencyChecker;
        UpgradeChecker = upgradeChecker;
        RemoveChecker = removeChecker;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public PluginScanSession<TAPlugin, TMeta> StartScan()
    {
        var token = Guid.NewGuid();
        var session = new PluginScanSession<TAPlugin, TMeta>(this, token);
        _activeSessions[token] = session;
        return session;
    }

    private void LoggerMetaSort(List<SortPluginData<TMeta>> sorts)
    {
        Logger?.Information("\nLoading Plugin Sort:\n" +
                            string.Join("\n", sorts.Select(meta => $" - {meta.Id}: {meta.MetaData.Version}")));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> FinishScanAsync(Task<SortPluginData<TMeta>>[] scanTaskArray, Guid token)
    {
        await _finishLock.WaitAsync();
        try
        {
            if (!_activeSessions.TryRemove(token, out _))
            {
                throw new InvalidOperationException("Invalid or expired scan token.");
            }

            List<string> results = [];
            if (!UpgradeChecker.UpgradeChecked || !RemoveChecker.RemoveChecked)
                throw new PluginScanException(
                    "You need to try CheckUpgradeAndRemoveAsync before FinishAsync");

            List<SortPluginData<TMeta>> beforeSorts = [.. await Task.WhenAll(scanTaskArray)];
            CheckSdkVersion(beforeSorts);
            var sortResult = DependencyChecker.DetermineLoadOrder(beforeSorts.ToList());
            LoggerMetaSort(sortResult.Result);
            await Task.WhenAll(sortResult.Result.Select(GetMainPluginType).ToArray());
            sortResult.Result.ForEach(t =>
            {
                DependencyChecker.LoadedMetas[t.Id] = t.MetaData;
                results.Add(t.Id);
            });
            return results;
        }
        finally
        {
            _finishLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await RemoveChecker.CheckRemoveAsync();
        await UpgradeChecker.CheckUpgradeAsync();
    }

    /// <inheritdoc />
    public void CheckSdkVersion(List<SortPluginData<TMeta>> metaList)
    {
        var version = new NuGetVersion(typeof(TMeta).Assembly.GetName().Version!);
        foreach (var meta in metaList.Where(meta => meta.MetaData.SdkVersion.Major < version.Major ||
                                                    meta.MetaData.SdkVersion.Minor < version.Minor))
        {
            throw new PluginScanException(
                $"{meta.Id} Sdk Version Not Match, Need {version.Major}.{version.Minor}.*, But {meta.MetaData.SdkVersion}");
        }
    }

    /// <summary>
    /// Pre Load
    /// </summary>
    /// <param name="sortPluginData"></param>
    /// <returns></returns>
    /// <exception cref="PluginScanException"></exception>
    protected async Task<Tuple<string, Type>> GetMainPluginType(SortPluginData<TMeta> sortPluginData)
    {
        var dllFilePath =
            Path.GetFullPath(Path.GetDirectoryName(sortPluginData.Path!)! + "/../" + sortPluginData.MetaData.DllName +
                             ".dll");
        if (!File.Exists(dllFilePath)) throw new PluginScanException($"Not Found {dllFilePath}");

        var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(dllFilePath);
        var assembly = asm.ForeignAssembly;

        sortPluginData.MetaData.LoadEntryPoint(MetaDataHelper.Properties, assembly);

        var types = assembly.GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(BaseConfig).IsAssignableFrom(t) &&
                        t.GetCustomAttribute<ObservableConfigAttribute>() is { FileName: { Length: > 0 } })
            .Select(type => Task.Run(() =>
            {
                var loadMethod = type.GetMethod("Load",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (loadMethod == null) return;
                DiFactory.Services.RegisterInstance(loadMethod.Invoke(null, null));
            }))
            .ToArray();
        // Load Config File
        await Task.WhenAll(types);
        DiFactory.Services.Register(typeof(TAPlugin), sortPluginData.MetaData.MainPlugin.EntryPointType,
            reuse: Reuse.Singleton,
            serviceKey: sortPluginData.Id, made: Parameters.Of.Type(_ => sortPluginData.MetaData));
        return new Tuple<string, Type>(sortPluginData.Id, sortPluginData.MetaData.MainPlugin.EntryPointType);
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
}