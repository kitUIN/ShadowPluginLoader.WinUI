namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginNotFoundException
/// </summary>
public class PluginNotFoundException : System.Exception
{
    /// <summary>
    /// PluginNotFoundException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginNotFoundException(string? message) : base(message)
    {
    }
}
