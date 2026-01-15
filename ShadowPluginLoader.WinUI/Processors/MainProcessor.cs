using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomExtensions.WinUI;
using DryIoc;
using NuGet.Versioning;
using Serilog;
using ShadowObservableConfig;
using ShadowObservableConfig.Attributes;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Products;
using ShadowPluginLoader.WinUI.Workpieces;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 基础插件工厂
/// </summary>
public partial class MainProcessor<TAPlugin, TMeta> : IMainProcessor
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    private readonly SemaphoreSlim _finishLock = new(1, 1);


    /// <summary>
    /// Logger
    /// </summary>
    private ILogger Logger { get; } = Log.ForContext("SourceContext", "BaseMainProcessor");

    /// <summary>
    /// DependencyChecker
    /// </summary>
    [Autowired]
    protected IDependencyChecker<TMeta> DependencyChecker { get; }

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected BaseSdkConfig BaseSdkConfig { get; }

    /// <summary>
    /// 
    /// </summary>
    protected async Task<List<SortPluginData<TMeta>>> ReadWorkpiece(
        IEnumerable<IWorkpiece> workpieces,
        IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var workpiecesArray = workpieces as IWorkpiece[] ?? workpieces.ToArray();
        var totalCount = workpiecesArray.Length;
        if (totalCount == 0) return [];

        // 限制最大并发数为 6
        using var semaphore = new SemaphoreSlim(6);

        // 已完全处理完成的计数
        var completedCount = 0;

        var tasks = workpiecesArray.Select(async (workpiece) =>
        {
            // 异步等待信号量
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var meta = MetaDataHelper.ToMeta<TMeta>(workpiece.MetaData);
                if (meta == null)
                {
                    Logger?.Warning("Scan Uri[{DirFullName}]: Read Meta Error", workpiece.Path);
                    Interlocked.Increment(ref completedCount);
                    // 即使失败，也同步一下总进度
                    ReportReadWorkpieceProgress(progress, completedCount, totalCount, "");
                    return null;
                }

                var path = workpiece.Path;

                if (workpiece is CompressedWorkpiece compressedWorkpiece)
                {
                    path = await ZipHelper.UnZip(
                        compressedWorkpiece.Path.LocalPath,
                        BaseSdkConfig.PluginFolderPath,
                        meta.DllName,
                        cancellationToken: cancellationToken);
                }

                var nowFinished = Interlocked.Increment(ref completedCount);
                ReportReadWorkpieceProgress(progress, nowFinished, totalCount);

                return new SortPluginData<TMeta>(meta, path);
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, "处理工作件失败: {Path}", workpiece.Path);
                var nowFinished = Interlocked.Increment(ref completedCount);
                ReportReadWorkpieceProgress(progress, nowFinished, totalCount);
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null).ToList()!;
    }


    private void ReportReadWorkpieceProgress(IProgress<PipelineProgress>? progress, int completed, int total,
        string status = "")
    {
        var percent = (double)completed / total;
        progress?.Report(new PipelineProgress(
            TotalPercentage: 0.33D + percent * 0.16D,
            TotalStatusValue: "",
            Step: InstallPipelineStep.MainProcessing,
            SubPercentage: percent,
            SubStatusValue: status,
            SubStep: SubInstallPipelineStep.ReadWorkpiece
        ));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IProduct>> MainProcessAsync(IEnumerable<IWorkpiece> workpieces,
        IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        await _finishLock.WaitAsync(cancellationToken);
        try
        {
            List<IProduct> results = [];
            if (!StaticValues.UpgradeChecked || !StaticValues.RemoveChecked)
                throw new PluginScanException(
                    "You need to try CheckUpgradeAndRemoveAsync before FinishScanAsync");
            var beforeSorts = await ReadWorkpiece(workpieces, progress, cancellationToken);
            progress?.Report(new PipelineProgress(
                TotalPercentage: 0.49D,
                TotalStatusValue: "",
                Step: InstallPipelineStep.MainProcessing,
                SubPercentage: 1D,
                SubStep: SubInstallPipelineStep.ReadWorkpiece
            ));
            CheckSdkVersion(beforeSorts);
            var sortResult = DependencyChecker.DetermineLoadOrder(beforeSorts.ToList());
            LoggerMetaSort(sortResult.Result);
            await GetMainPluginTypeTask(sortResult.Result.ToArray(), progress);
            sortResult.Result.ForEach(t =>
            {
                DependencyChecker.LoadedMetas[t.Id] = t.MetaData;
                results.Add(new BaseProduct(t.Id));
            });
            return results;
        }
        finally
        {
            _finishLock.Release();
        }
    }

    private void LoggerMetaSort(IEnumerable<SortPluginData<TMeta>> sorts)
    {
        Logger?.Information("\nLoading Plugin Sort:\n" +
                            string.Join("\n", sorts.Select(meta => $" - {meta.Id}: {meta.MetaData.Version}")));
    }


    private void CheckSdkVersion(IEnumerable<SortPluginData<TMeta>> metaList)
    {
        var version = new NuGetVersion(typeof(TMeta).Assembly.GetName().Version!);
        foreach (var meta in metaList.Where(meta => !meta.MetaData.SdkVersion.Satisfies(version)))
        {
            throw new PluginScanException(
                $"{meta.Id} Sdk Version Not Match, Need {meta.MetaData.SdkVersion}, But {version}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginDataList"></param>
    /// <param name="progress"></param>
    private async Task GetMainPluginTypeTask(SortPluginData<TMeta>[] pluginDataList,
        IProgress<PipelineProgress>? progress = null)
    {
        int totalPlugins = pluginDataList.Length;

        if (totalPlugins == 0) return;

        var completedPlugins = 0;
        var weight = 100.0 / totalPlugins;

        var tasks = pluginDataList.Select(async (data) =>
        {
            try
            {
                // 1. 执行原有的逻辑
                await GetMainPluginType(data);
            }
            finally
            {
                // 2. 无论成功失败，增加完成计数
                var nowFinished = Interlocked.Increment(ref completedPlugins);

                // 3. 汇报进度
                var currentPercent = nowFinished * weight;
                progress?.Report(new PipelineProgress(
                    TotalPercentage: 0.49D + currentPercent * 0.16D,
                    Step: InstallPipelineStep.MainProcessing,
                    SubPercentage: currentPercent,
                    SubStep: SubInstallPipelineStep.PluginPreLoad
                ));
            }
        });
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Pre Load
    /// </summary>
    /// <exception cref="PluginScanException"></exception>
    protected async Task<Tuple<string, Type>> GetMainPluginType(SortPluginData<TMeta> sortPluginData)
    {
        var dllFileUri = new Uri(sortPluginData.Link, "../" + sortPluginData.MetaData.DllName + ".dll");
        var dllFilePath = dllFileUri.LocalPath;
        if (!File.Exists(dllFilePath)) throw new PluginScanException($"Not Found {dllFilePath}");

        var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(dllFilePath);
        var assembly = asm.ForeignAssembly;

        sortPluginData.MetaData.LoadEntryPoint(MetaDataHelper.Properties!, assembly);

        var types = assembly.GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(BaseConfig).IsAssignableFrom(t) &&
                        t.GetCustomAttribute<ObservableConfigAttribute>() is { FileName: { Length: > 0 } })
            .Select(type => Task.Run(() =>
            {
                try
                {
                    var loadMethod = type.GetMethod("Load",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (loadMethod == null) return;
                    var instance = loadMethod.Invoke(null, null);
                    DiFactory.Services.RegisterInstance(type, instance);
                    Logger?.Information("{Plugin} Load Success: {Type}", sortPluginData.Id, type.FullName);
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex, "{Plugin} Failed To Load: {Type}", sortPluginData.Id, type.FullName);
                }
            }))
            .ToArray();
        // Load Config File
        await Task.WhenAll(types);
        DiFactory.Services.Register(typeof(TAPlugin), sortPluginData.MetaData.MainPlugin.EntryPointType,
            reuse: Reuse.Singleton,
            serviceKey: sortPluginData.Id, made: Parameters.Of.Type(_ => sortPluginData.MetaData));
        return new Tuple<string, Type>(sortPluginData.Id, sortPluginData.MetaData.MainPlugin.EntryPointType);
    }
}