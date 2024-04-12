namespace ShadowPluginLoader.WinUI.Exceptions;

public class PluginNotFoundException : System.Exception
{
    public PluginNotFoundException() : base() { }

    public PluginNotFoundException(string? message) : base(message) { }

    public PluginNotFoundException(string? message, System.Exception? innerException) : base(message, innerException) { }

}
