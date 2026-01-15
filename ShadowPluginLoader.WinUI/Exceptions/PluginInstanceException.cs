namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginInstanceException
/// </summary>
public class PluginInstanceException : System.Exception
{
    /// <summary>
    /// PluginInstanceException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginInstanceException(string? message) : base(message)
    {
    }
}
