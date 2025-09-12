using System;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract PluginMetaData
/// </summary>
public abstract record AbstractPluginMetaData : IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true)]
    public string Id { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.BuiltIn"/>
    /// </summary>
    [Meta(Exclude = true)]
    public bool BuiltIn { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.DllName"/>
    /// </summary>
    [Meta(Required = false)]
    public string DllName { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.DllPath"/>
    /// </summary>
    [Meta(Required = false)]
    public string DllPath { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true, JsonType = typeof(string),
        ConstructionTemplate = "new System.Version(\"{{ RAW }}\")")]
    public Version Version { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.MainPluginType"/>
    /// </summary>
    [Meta(Exclude = true)]
    public Type MainPluginType { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Priority"/>
    /// </summary>
    [Meta(Required = false)]
    public int Priority { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Dependencies"/>
    /// </summary>
    [Meta(Required = false, JsonType = typeof(string[]),
        ConstructionTemplate = "new ShadowPluginLoader.WinUI.Models.PluginDependency(\"{{ RAW }}\")")]
    public PluginDependency[] Dependencies { get; init; } = [];

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.EntryPoints"/>
    /// </summary>
    [Meta(Exclude = true)]
    public PluginEntryPoint[] EntryPoints { get; init; } = [];
}