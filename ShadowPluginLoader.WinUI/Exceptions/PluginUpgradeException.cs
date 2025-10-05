namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// 
/// </summary>
public class PluginUpgradeException : System.Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginUpgradeException(string? message) : base(message)
    {
    }
}