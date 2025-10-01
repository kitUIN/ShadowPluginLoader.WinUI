using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// Remove Checker
/// </summary>
public interface IRemoveChecker
{
    /// <summary>
    /// Is Remove Checked
    /// </summary>
    bool RemoveChecked { get; }

    /// <summary>
    /// Check Remove Async
    /// </summary>
    /// <returns></returns>
    Task<bool>  CheckRemoveAsync();
}