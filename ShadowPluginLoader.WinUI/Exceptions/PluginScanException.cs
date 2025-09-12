namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginScanException
/// </summary>
public class PluginScanException : System.Exception
{
    /// <summary>
    /// PluginScanException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginScanException(string? message) : base(message)
    {
    }
}