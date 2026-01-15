using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Products;

namespace ShadowPluginLoader.WinUI.Pipelines;

/// <summary>
/// 流水线
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// 流水线编号
    /// </summary>
    Guid Uuid { get; }

    /// <summary>
    /// 投入原料
    /// </summary>
    /// <param name="material">投入原料</param>
    /// <returns></returns>
    IPipeline Feed(IMaterial material);

    /// <summary>
    /// 加工
    /// </summary>
    /// <returns>产品</returns>
    Task ProcessAsync(IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default);
}