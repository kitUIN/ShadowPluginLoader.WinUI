namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Default Plugin Interface
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Get identifier of this plugin 
    /// </summary>
    string GetId();

    /// <summary>
    /// Get meta data of this plugin
    /// </summary>
    IPluginMetaData GetMetaData();

    /// <summary>
    /// Know that this Plugin is Enabled/Disabled
    /// </summary>
    bool IsEnabled { get; set; }
    /// <summary>
    /// Enable this Plugin
    /// </summary>
    void Enable();
    /// <summary>
    /// Disable this Plugin
    /// </summary>
    void Disable();
}
