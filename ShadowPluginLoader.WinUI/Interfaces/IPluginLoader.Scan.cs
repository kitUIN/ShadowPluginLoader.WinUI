using ShadowPluginLoader.WinUI.Installer;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Scanners;

namespace ShadowPluginLoader.WinUI.Interfaces;

public partial interface IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// StartScan
    /// </summary>
    /// <returns></returns>
    PluginScanSession<TAPlugin, TMeta> StartScan();
}