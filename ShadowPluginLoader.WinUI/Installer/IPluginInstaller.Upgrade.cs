using ShadowPluginLoader.WinUI.Exceptions;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Installer;

/// <summary>
/// 
/// </summary>
public partial interface IPluginInstaller<TMeta>  
{


    /// <summary>
    /// PlanUpgrade
    /// <exception cref="PlanUpgradeException"></exception>
    /// </summary>
    void PlanUpgrade(string id, string pluginPath, string zipPath);

    /// <summary>
    /// Revoked Upgrade
    /// </summary>
    /// <param name="id"></param>
    void RevokedUpgrade(string id);

    /// <summary>
    /// Check Upgrade Async
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckUpgradeAsync();
}