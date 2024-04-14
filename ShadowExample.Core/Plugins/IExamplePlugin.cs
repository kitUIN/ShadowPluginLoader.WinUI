using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Core.Plugins;

public interface IExamplePlugin: IPlugin
{
    string GetEmoji();
}