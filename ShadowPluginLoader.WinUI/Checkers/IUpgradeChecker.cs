using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// Plugin Upgrade Checker
/// </summary>
public interface IUpgradeChecker
{
    /// <summary>
    /// Is Upgrade Checked
    /// </summary>
    bool UpgradeChecked { get; }

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