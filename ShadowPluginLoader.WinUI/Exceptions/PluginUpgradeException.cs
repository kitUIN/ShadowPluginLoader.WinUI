
namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginUpgradeException
/// </summary>
public class PluginUpgradeException : System.Exception
{
    /// <summary>
    /// PluginUpgradeException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginUpgradeException(string? message) : base(message)
    {
    }
}