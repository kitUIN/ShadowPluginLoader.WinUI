using System;
using ShadowPluginLoader.MetaAttributes;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Abstract PluginMetaData
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public abstract class AbstractPluginMetaData: Attribute, IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true, PropertyGroupName = "PackageId")]
    public string Id { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true, PropertyGroupName = "PluginName")]
    public string Name { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true)]
    public string Version { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Dependencies"/>
    /// </summary>
    [Meta(Required = true)]
    public string[] Dependencies { get; init; }
}