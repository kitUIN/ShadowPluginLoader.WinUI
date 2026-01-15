using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.PluginFactories;
using ShadowPluginLoader.WinUI.Processors;
using ShadowPluginLoader.WinUI.Products;

namespace ShadowPluginLoader.WinUI.Pipelines;

/// <summary>
/// 基础流水线
/// </summary>
public partial class InstallPipeline : IPipeline
{
    /// <summary>
    /// 主处理器
    /// </summary>
    [Autowired]
    private IMainProcessor MainProcessor { get; }

    /// <summary>
    /// 工厂
    /// </summary>
    [Autowired]
    private IPluginFactory PluginFactory { get; }


    /// <inheritdoc/>
    public Guid Uuid { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public IPipeline Feed(IMaterial material)
    {
        Materials.Add(material);
        return this;
    }

    /// <summary>
    /// 原料
    /// </summary>
    public List<IMaterial> Materials { get; } = [];

    /// <summary>
    /// 产出
    /// </summary>
    public List<IProduct> Products { get; } = [];

    /// <inheritdoc/>
    public async Task ProcessAsync(IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var materials = Materials.ToArray();
        Materials.Clear();
        Products.Clear();
        var completedCount = 0;
        var totalMaterials = materials.Length;
        if (totalMaterials == 0) return;
        var tasks = materials.Select(async material =>
        {
            if (!ProcessorRegistry.PreprocessingProcessors.TryGetValue(material.TypeName, out var preprocessor))
            {
                return null;
            }

            var result = await preprocessor
                .PreprocessAsync(material, cancellationToken);
            var currentFinished = Interlocked.Increment(ref completedCount);
            var subPercent = (double)currentFinished / totalMaterials;
            progress?.Report(new PipelineProgress(
                TotalPercentage: subPercent * 0.33D,
                TotalStatusValue: "",
                Step: InstallPipelineStep.Preprocessing,
                SubPercentage: subPercent,
                SubStatusValue: material.TypeName,
                SubStep: SubInstallPipelineStep.None
            ));

            return result;
        });

        var workpieces = (await Task.WhenAll(tasks)).Where(x => x != null).ToArray();
        if (workpieces.Length == 0) return;
        foreach (var product in await MainProcessor.MainProcessAsync(workpieces!, progress, cancellationToken))
        {
            Products.Add(product);
        }

        if (Products.Count == 0) return;
        var count = PluginFactory.Outbound(Products, progress);
        progress?.Report(new PipelineProgress(
            TotalPercentage: 1D,
            TotalStatusValue: count.ToString(),
            Step: InstallPipelineStep.Success
        ));
    }
}