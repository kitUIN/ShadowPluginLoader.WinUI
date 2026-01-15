using System;
using System.Collections.Generic;
using System.Linq;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Pipelines;
using ShadowPluginLoader.WinUI.PluginFactories;
using ShadowPluginLoader.WinUI.Processors;
using ShadowPluginLoader.WinUI.Products;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin> : IPluginFactory
{
    /// <summary>
    /// 主加载器
    /// </summary>
    [Autowired]
    private IMainProcessor MainProcessor { get; }

    /// <inheritdoc />
    public IPipeline CreatePipeline()
    {
        return new InstallPipeline(MainProcessor, this);
    }

    /// <inheritdoc />
    public int Outbound(IEnumerable<IProduct> products, IProgress<PipelineProgress>? progress = null)
    {
        return Load(products.Select(product => product.Id), progress);
    }
}