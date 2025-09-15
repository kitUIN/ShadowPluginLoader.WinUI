using NuGet.Versioning;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Plugin Dependency
/// </summary>
public record PluginDependency
{
    /// <summary>
    /// 
    /// </summary>
    public PluginDependency(string id, string name, string versionRange)
    {
        Id = id;
        Name = name;
        Need = VersionRange.Parse(versionRange);
    }

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public VersionRange Need { get; init; }


    /// <inheritdoc />
    public override string ToString()
    {
        return Id + Need;
    }
}