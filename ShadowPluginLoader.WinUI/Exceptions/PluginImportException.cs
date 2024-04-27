namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginImportException
/// </summary>
public class PluginImportException : System.Exception
{
    /// <summary>
    /// PluginImportException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginImportException(string? message) : base(message)
    {
    }
}