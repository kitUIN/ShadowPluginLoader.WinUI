using Microsoft.UI.Xaml;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase: AbstractPlugin
{
    public abstract string GetEmoji();
    public abstract FrameworkElement GetControl();
}