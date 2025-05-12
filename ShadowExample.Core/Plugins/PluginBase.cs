using Serilog;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase : AbstractPlugin<ExampleMetaData>
{
    /// <inheritdoc />
    protected PluginBase(ILogger logger, PluginEventService pluginEventService) : base(logger, pluginEventService)
    {
    }
}