using NuGet.Versioning;
using ShadowPluginLoader.WinUI.Models;
using System;

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
    NuGetVersion Version { get; init; }

    /// <summary>
    /// Plugin Type
    /// </summary>
    Type MainPluginType { get; init; }

    /// <summary>
    /// Plugin Dependencies
    /// </summary>
    PluginDependency[] Dependencies { get; init; }

    /// <summary>
    /// Loading Order Priority
    /// Determines the execution order of the plugin. 
    /// Plugins with lower values are executed earlier.
    /// </summary>
    int Priority { get; init; }

    /// <summary>
    /// EntryPoints
    /// </summary>
    PluginEntryPoint[] EntryPoints { get; init; }

    /// <summary>
    /// BuiltIn Plugin
    /// </summary>
    bool BuiltIn { get; init; }
}