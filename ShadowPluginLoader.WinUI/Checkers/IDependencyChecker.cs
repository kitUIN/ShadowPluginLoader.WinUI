using NuGet.Versioning;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;

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
    Dictionary<string, NuGetVersion> LoadedPlugins { get; }

    /// <summary>
    /// LoadedMetas
    /// </summary>
    Dictionary<string, TMeta> LoadedMetas { get; }

    /// <summary>
    /// DetermineLoadOrder
    /// </summary>
    /// <param name="plugins"></param>
    /// <returns></returns>
    DependencyCheckResult<TMeta> DetermineLoadOrder(IEnumerable<SortPluginData<TMeta>> plugins);
}