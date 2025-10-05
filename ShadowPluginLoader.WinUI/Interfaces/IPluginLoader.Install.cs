using ShadowPluginLoader.WinUI.Installer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces;

public partial interface IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// Install From .sdow file
    /// </summary>
    Task InstallAsync(IEnumerable<string> shadowFiles);
}