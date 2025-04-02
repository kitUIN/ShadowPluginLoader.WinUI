using System.Text.Json.Nodes;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Interfaces;

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
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true)]
    public string Version { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = false)]
    public int Priority { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Dependencies"/>
    /// </summary>
    [Meta(Required = false)]
    public string[] Dependencies { get; init; } = [];

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.EntryPoints"/>
    /// </summary>
    [Meta(Exclude = true)]
    public JsonNode? EntryPoints { get; init; } = null;
}