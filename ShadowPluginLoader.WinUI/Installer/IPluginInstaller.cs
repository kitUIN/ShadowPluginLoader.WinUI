using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Installer;

/// <summary>
/// 
/// </summary>
public interface IPluginInstaller
{
    /// <summary>
    /// Install From .sdow file
    /// </summary>
    /// <param name="shadowFiles"></param>
    /// <exception cref="PluginInstallException"></exception>
    /// <returns></returns>
    Task<IEnumerable<string>> InstallAsync(IEnumerable<string> shadowFiles);
}