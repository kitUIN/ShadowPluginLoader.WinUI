namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Default PluginMetaData Interface
/// </summary>
public interface IPluginMetaData
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Plugin Name
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Plugin Version
    /// </summary>
    string Version { get; }
    /// <summary>
    /// Plugin Requires
    /// </summary>
    string[] Requires { get; }
}