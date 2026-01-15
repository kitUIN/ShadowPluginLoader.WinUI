using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Products;
using ShadowPluginLoader.WinUI.Workpieces;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 正式加工处理器
/// </summary>
public interface IMainProcessor
{
    /// <summary>
    /// 正式加工
    /// </summary> 
    /// <returns></returns>
    Task<IEnumerable<IProduct>> MainProcessAsync(IEnumerable<IWorkpiece> workpieces,
        IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default);
}