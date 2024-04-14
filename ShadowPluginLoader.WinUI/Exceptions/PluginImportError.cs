namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginImportError
/// </summary>
public class PluginImportError : System.Exception
{
    /// <summary>
    /// PluginImportError
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginImportError(string? message) : base(message)
    {
    }
}