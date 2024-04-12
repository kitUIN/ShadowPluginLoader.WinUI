namespace ShadowPluginLoader.WinUI.Exceptions;

public class PluginLoadError : System.Exception
{
    public PluginLoadError() : base() { }

    public PluginLoadError(string? message) : base(message) { }

    public PluginLoadError(string? message, System.Exception? innerException) : base(message, innerException) { }

}
