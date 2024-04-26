using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Default Plugin Interface
/// </summary>
internal interface IPlugin
{
    /// <summary>
    /// Get identifier of this plugin 
    /// </summary>
    string GetId();
    /// <summary>
    /// Know that this Plugin is Enabled/Disabled
    /// </summary>
    bool IsEnabled { get; set; }
}
