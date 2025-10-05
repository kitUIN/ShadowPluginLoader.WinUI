namespace ShadowPluginLoader.WinUI.Exceptions;

/// <summary>
/// PluginDependencyException
/// </summary>
public class PluginDependencyException : System.Exception
{
    /// <summary>
    /// PluginDependencyException
    /// </summary>
    /// <param name="message">Error Message</param>
    public PluginDependencyException(string? message) : base(message)
    {
    }
}