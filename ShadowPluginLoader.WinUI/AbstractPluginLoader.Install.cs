using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Installer;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// PluginInstaller
    /// </summary>
    protected IPluginInstaller PluginInstaller { get; }

    /// <inheritdoc />
    public async Task InstallAsync(IEnumerable<string> shadowFiles)
    {
       var result = await PluginInstaller.InstallAsync(shadowFiles);
       Load(result);
    }
}