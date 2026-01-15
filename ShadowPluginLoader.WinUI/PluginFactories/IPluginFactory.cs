using System;
using System.Collections.Generic;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Pipelines;
using ShadowPluginLoader.WinUI.Products;

namespace ShadowPluginLoader.WinUI.PluginFactories;

/// <summary>
/// 插件工厂接口
/// </summary>
public interface IPluginFactory
{
    /// <summary>
    /// 创建流水线
    /// </summary>
    /// <returns></returns>
    IPipeline CreatePipeline();

    /// <summary>
    /// 出库(实例化插件)
    /// </summary>
    int Outbound(IEnumerable<IProduct> products, IProgress<PipelineProgress>? progress = null);
}