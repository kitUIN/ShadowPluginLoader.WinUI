namespace ShadowPluginLoader.WinUI.Interfaces;
using MetaAttributes;
/// <summary>
/// Default PluginMetaData Interface
/// </summary>
internal interface IPluginMetaData
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    string Id { get; init; }
    /// <summary>
    /// Plugin Name
    /// </summary>
    string Name { get; init; }
    /// <summary>
    /// Plugin Version
    /// </summary>
    string Version { get; init; }
    /// <summary>
    /// Plugin Dependencies
    /// </summary>
    string[] Dependencies { get; init; }
}