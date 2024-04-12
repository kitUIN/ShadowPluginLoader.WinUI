namespace ShadowPluginLoader.WinUI.Exceptions;

public class PluginMetaNotFoundException : System.Exception
{
    public PluginMetaNotFoundException() : base() { }

    public PluginMetaNotFoundException(string? message) : base(message) { }

    public PluginMetaNotFoundException(string? message, System.Exception? innerException) : base(message, innerException) { }

}
