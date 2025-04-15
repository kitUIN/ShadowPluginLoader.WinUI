using System.Text.Json.Nodes;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Default PluginMetaData Interface
/// </summary>
internal interface IPluginMetaData
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    string Id { get; init; }

    /// <summary>
    /// Plugin Name
    /// </summary>
    string Name { get; init; }

    /// <summary>
    /// Plugin DllName
    /// </summary>
    string DllName { get; init; }

    /// <summary>
    /// Plugin Version
    /// </summary>
    string Version { get; init; }

    /// <summary>
    /// Plugin Dependencies
    /// </summary>
    PluginDependency[] Dependencies { get; init; }

    /// <summary>
    /// Loading Order Priority
    /// </summary>
    int Priority { get; init; }

    /// <summary>
    /// EntryPoints
    /// </summary>
    JsonNode? EntryPoints { get; init; }

    /// <summary>
    /// BuiltIn Plugin
    /// </summary>
    bool BuiltIn { get; init; }
}