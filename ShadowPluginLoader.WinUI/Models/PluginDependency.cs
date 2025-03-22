namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// 
/// </summary>
public class PluginDependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="version"></param>
    public PluginDependency(string id, string? version=null)
    {
        Id = id;
        Version = version;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? Version { get; set; }

}