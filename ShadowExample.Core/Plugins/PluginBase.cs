using Microsoft.UI.Xaml;
using Serilog;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase : AbstractPlugin
{
    /// <inheritdoc />
    protected PluginBase(ILogger logger, PluginEventService pluginEventService) : base(logger, pluginEventService)
    {
    }

    public abstract string GetEmoji();
    public abstract FrameworkElement GetControl();
}