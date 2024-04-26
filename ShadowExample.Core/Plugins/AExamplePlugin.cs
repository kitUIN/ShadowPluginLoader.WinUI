using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Core.Plugins;

public abstract class AExamplePlugin: APlugin
{
    public abstract string GetEmoji();
}