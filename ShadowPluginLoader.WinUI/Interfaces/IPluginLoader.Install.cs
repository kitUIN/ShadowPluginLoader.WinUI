using ShadowPluginLoader.WinUI.Installer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces;

public partial interface IPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// Install From .sdow file
    /// </summary>
    Task InstallAsync(IEnumerable<string> shadowFiles);

    /// <summary>
    /// Checked for updates and removed plugins
    /// </summary>
    /// <returns></returns>
    Task CheckUpgradeAndRemoveAsync();

    /// <summary>
    /// Remove Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    Task RemovePlugin(string id);
}