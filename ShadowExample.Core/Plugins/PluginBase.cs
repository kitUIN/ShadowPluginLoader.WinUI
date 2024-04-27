using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase: AbstractPlugin
{
    public abstract string GetEmoji();
}