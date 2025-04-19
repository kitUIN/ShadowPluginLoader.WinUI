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
    Dictionary<string, string> LoadedPlugins { get; }

    /// <summary>
    /// DetermineLoadOrder
    /// </summary>
    /// <param name="plugins"></param>
    /// <returns></returns>
    List<SortPluginData<TMeta>> DetermineLoadOrder(List<SortPluginData<TMeta>> plugins);
}