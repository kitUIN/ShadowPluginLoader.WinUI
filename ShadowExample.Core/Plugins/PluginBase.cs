using Serilog;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase : AbstractPlugin<ExampleMetaData>
{
    /// <inheritdoc />
    protected PluginBase(ExampleMetaData meta, ILogger logger, PluginEventService pluginEventService) : base(meta,
        logger, pluginEventService)
    {
    }
}