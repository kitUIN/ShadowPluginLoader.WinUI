using System;
using System.Collections.Generic;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Dependency Checker
/// </summary>
/// <typeparam name="TMeta"></typeparam>
public interface IDependencyChecker<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// LoadedPluginDependencies
    /// </summary>
    Dictionary<string, Version> LoadedPlugins { get; }

    /// <summary>
    /// DetermineLoadOrder
    /// </summary>
    /// <param name="plugins"></param>
    /// <returns></returns>
    DependencyCheckResult<TMeta> DetermineLoadOrder(List<SortPluginData<TMeta>> plugins);
}