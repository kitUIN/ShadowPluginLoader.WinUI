namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginInstallException
/// </summary>
public class PluginInstallException : System.Exception
{
    /// <summary>
    /// PluginImportException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginInstallException(string? message) : base(message)
    {
    }
}