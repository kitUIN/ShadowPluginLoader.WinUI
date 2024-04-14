using System;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Default PluginMetaData
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DefaultPluginMetaData: Attribute, IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    public string Version { get; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Requires"/>
    /// </summary>
    public string[] Requires { get; }

    /// <summary>
    /// Default PluginMetaData
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="requires"></param>
    public DefaultPluginMetaData(string id, string name, string version, string[] requires)
    {
        Id = id;
        Name = name;
        Version = version;
        Requires = requires;
    }
}