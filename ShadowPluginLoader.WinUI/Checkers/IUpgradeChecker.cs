using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// 
/// </summary>
public interface IUpgradeChecker
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pluginPath"></param>
    /// <param name="zipPath"></param>
    void PlanUpgrade(string id, string pluginPath, string zipPath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    void RevokedUpgrade(string id);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckUpgradeAsync();
}