using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Args;

/// <summary>
/// Plugin Event Args
/// </summary>
public record PluginEventArgs
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    public string PluginId { get; init; }
    /// <summary>
    /// Plugin Status
    /// </summary>
    public PluginStatus Status { get; init; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">PluginId</param>
    /// <param name="status">PluginStatus</param>
    public PluginEventArgs(string id, PluginStatus status)
    {
        PluginId = id;
        Status = status;
    }
}
