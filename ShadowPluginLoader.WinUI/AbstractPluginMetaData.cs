using NuGet.Versioning;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Converters;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;

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
    [Meta(Exclude = true)]
    public string DllName { get; init; } = null!;


    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true, AsString = true, Converter = typeof(NuGetVersionJsonConverter))]
    public NuGetVersion Version { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.SdkVersion"/>
    /// </summary>
    [Meta(Required = false, AsString = true, Converter = typeof(NuGetVersionJsonConverter))]
    public NuGetVersion SdkVersion { get; init; } = null!;

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
    [Meta(Exclude = true, AsString = true, Converter = typeof(PluginDependencyJsonConverter))]
    public PluginDependency[] Dependencies { get; init; } = [];

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.EntryPoints"/>
    /// </summary>
    [Meta(Exclude = true)]
    public PluginEntryPoint[] EntryPoints { get; init; } = [];
}