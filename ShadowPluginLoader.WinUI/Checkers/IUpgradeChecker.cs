using System.Threading.Tasks;

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
    /// Check Upgrade Async
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckUpgradeAsync();
}