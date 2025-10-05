using NuGet.Versioning;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// Dependency Checker
/// </summary>
/// <typeparam name="TMeta"></typeparam>
public interface IDependencyChecker<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// LoadedPluginDependencies
    /// </summary>
    ConcurrentDictionary<string, NuGetVersion> LoadedPlugins { get; }

    /// <summary>
    /// LoadedMetas
    /// </summary>
    ConcurrentDictionary<string, TMeta> LoadedMetas { get; }

    /// <summary>
    /// CheckUpgrade
    /// </summary>
    /// <exception cref="PluginUpgradeException"></exception>
    /// <exception cref="PluginDependencyException"></exception>
    Task CheckUpgrade(string id, Uri uri);

    /// <summary>
    /// DetermineLoadOrder
    /// </summary>
    /// <exception cref="PluginDependencyException"></exception>
    DependencyCheckResult<TMeta> DetermineLoadOrder(IEnumerable<SortPluginData<TMeta>> plugins);
}