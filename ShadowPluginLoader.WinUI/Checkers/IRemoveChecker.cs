using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// Remove Checker
/// </summary>
public interface IRemoveChecker
{

    /// <summary>
    /// Plan Remove
    /// <exception cref="PlanRemoveException"></exception>
    /// </summary>
    void PlanRemove(string id, string path);

    /// <summary>
    /// Revoked Remove
    /// </summary>
    /// <param name="id"></param>
    void RevokedRemove(string id);

    /// <summary>
    /// Check Remove Async
    /// </summary>
    /// <returns></returns>
    Task<bool> CheckRemoveAsync();
}