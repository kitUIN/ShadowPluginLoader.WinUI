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
    /// Get Or Set Plugin Enabled/Disabled
    /// <remarks>
    /// It Will Call Enabled/Disabled Function And Plugin Enabled/Disabled Event
    /// </remarks>
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Plugin Loaded (Before Plugin Loaded Event) (Before Plugin Enable Event)
    /// </summary>
    void Loaded();
}
