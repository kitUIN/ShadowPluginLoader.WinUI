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
    string Id { get; }
    /// <summary>
    /// Get display name of this plugin 
    /// </summary>
    string DisplayName { get; }
    /// <summary>
    /// Know that this Plugin is Enabled/Disabled
    /// </summary>
    bool IsEnabled { get; set; }
}
