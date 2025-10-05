namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginLoadMetaException
/// </summary>
public class PluginLoadMetaException : System.Exception
{
    /// <summary>
    /// PluginLoadMetaException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginLoadMetaException(string? message) : base(message)
    {
    }
}