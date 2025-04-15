using System;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Plugin Dependency Comparer
/// </summary>
public enum PluginDependencyComparer
{
    /// <summary>
    /// =
    /// </summary>
    Same,
    /// <summary>
    /// &gt;=
    /// </summary>
    Greater,
    /// <summary>
    /// &lt;=
    /// </summary>
    Lesser,
}

/// <summary>
/// Plugin Dependency
/// </summary>
public record PluginDependency
{
    /// <summary>
    /// 
    /// </summary>
    public PluginDependency(string raw)
    {
        string comparer;
        if (raw.Contains(">="))
        {
            comparer = ">=";
        }else if (raw.Contains("<="))
        {
            comparer = "<=";
        }
        else
        {
            comparer = "=";
        }
        var rawParts = raw.Split(comparer);
        if(rawParts.Length != 2) throw new Exception($"Invalid plugin dependency: {raw}");
        Id = rawParts[0];
        Version = new Version(rawParts[1]);
        Need = comparer + rawParts[1];
        Comparer = comparer switch
        {
            "=" => PluginDependencyComparer.Same,
            "<=" => PluginDependencyComparer.Lesser,
            _ => PluginDependencyComparer.Greater
        };
    }
    /// <summary>
    /// 
    /// </summary>
    public PluginDependency(string id, string version, string comparer)
    {
        Id = id;
        Version = new Version(version);
        Need = comparer + version;
        Comparer = comparer switch
        {
            "=" => PluginDependencyComparer.Same,
            "<=" => PluginDependencyComparer.Lesser,
            _ => PluginDependencyComparer.Greater
        };
    }
    /// <summary>
    /// 
    /// </summary>
    public string Id { get; init; }    
    /// <summary>
    /// 
    /// </summary>
    public string Need { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public Version Version { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public PluginDependencyComparer Comparer { get; init; }
}