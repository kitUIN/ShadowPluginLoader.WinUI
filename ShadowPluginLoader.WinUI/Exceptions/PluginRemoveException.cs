
namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginRemoveException
/// </summary>
public class PluginRemoveException : System.Exception
{
    /// <summary>
    /// PluginRemoveException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginRemoveException(string? message) : base(message)
    {
    }
}