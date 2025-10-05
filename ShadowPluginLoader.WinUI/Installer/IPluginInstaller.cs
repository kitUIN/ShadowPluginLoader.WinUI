using ShadowPluginLoader.WinUI.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Installer;

/// <summary>
/// 
/// </summary>
public partial interface IPluginInstaller<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// Install From .sdow file
    /// </summary>
    /// <param name="shadowFiles"></param>
    /// <exception cref="PluginInstallException"></exception>
    /// <returns></returns>
    Task<List<SortPluginData<TMeta>>> InstallAsync(IEnumerable<string> shadowFiles);
}