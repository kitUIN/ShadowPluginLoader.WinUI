using System;
using ShadowPluginLoader.MetaAttributes;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowPluginLoader.WinUI;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

/// <summary>
/// Abstract PluginMetaData
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public abstract class AbstractPluginMetaData : Attribute, IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true)]
    public string Id { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true)]
    public string Name { get; init; }
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.DllName"/>
    /// </summary>
    [Meta(Required = false)]
    public string DllName { get; init; }
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
