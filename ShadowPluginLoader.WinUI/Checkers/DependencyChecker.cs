using ShadowPluginLoader.WinUI.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// 
/// </summary>
public class DependencyChecker<TMeta> : IDependencyChecker<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public Dictionary<string, Version> LoadedPlugins { get; } = new();


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public DependencyCheckResult<TMeta> DetermineLoadOrder(List<SortPluginData<TMeta>> plugins)
    {
        var sortedPlugins = new List<SortPluginData<TMeta>>();
        var visited = new HashSet<string>();
        var needUpgradePlugins = new List<SortPluginData<TMeta>>();
        var uniquePlugins = plugins
            .GroupBy(p => p.Id)
            .Select(g => g
                .OrderByDescending(p => p.Version)
                .ThenBy(p => p.Priority)
                .First())
            .Where(p =>
            {
                if (!LoadedPlugins.ContainsKey(p.Id)) return true;
                if (LoadedPlugins.TryGetValue(p.Id, out var actualVersion) &&
                    actualVersion < p.Version)
                    needUpgradePlugins.Add(p);
                return false;
            })
            .OrderBy(p => p.Priority)
            .ToList();

        foreach (var plugin in uniquePlugins)
        {
            SortDependencies(plugin, uniquePlugins, sortedPlugins, visited);
        }

        return new DependencyCheckResult<TMeta>(sortedPlugins, needUpgradePlugins);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plugin"></param>
    /// <param name="plugins"></param>
    /// <param name="sortedPlugins"></param>
    /// <param name="visited"></param>
    /// <exception cref="PluginImportException"></exception>
    private void SortDependencies(SortPluginData<TMeta> plugin, List<SortPluginData<TMeta>> plugins,
        List<SortPluginData<TMeta>> sortedPlugins, HashSet<string> visited)
    {
        visited.Add(plugin.Id);

        foreach (var dependency in plugin.Dependencies)
        {
            var dependentPlugin = plugins.FirstOrDefault(p => p.Id == dependency.Id);

            if (dependentPlugin == null)
            {
                if (!LoadedPlugins.TryGetValue(dependency.Id, out var loadedPlugin))
                    throw new PluginImportException($"Dependency Not Found: {dependency.Id}");
                if (!IsVersionSatisfied(loadedPlugin, dependency))
                {
                    throw new PluginImportException(
                        $"Version Not Satisfied: {dependency.Id}, Need: {dependency.Need}, Actual: {loadedPlugin}");
                }

                continue;
            }

            if (!IsVersionSatisfied(dependentPlugin.Version, dependency))
            {
                throw new PluginImportException(
                    $"Version Not Satisfied: {dependentPlugin.Id}, Need: {dependency.Need}, Actual: {dependentPlugin.Version}");
            }

            if (!visited.Contains(dependentPlugin.Id))
            {
                SortDependencies(dependentPlugin, plugins, sortedPlugins, visited);
            }
        }

        if (!sortedPlugins.Contains(plugin))
        {
            sortedPlugins.Add(plugin);
        }
    }


    /// <summary>
    /// Check Version Satisfied
    /// </summary>
    /// <param name="actualVersion"></param>
    /// <param name="dependency"></param>
    /// <returns></returns>
    private bool IsVersionSatisfied(Version actualVersion, PluginDependency dependency)
    {
        return dependency.Comparer switch
        {
            PluginDependencyComparer.Lesser => actualVersion <= dependency.Version,
            PluginDependencyComparer.Same => actualVersion == dependency.Version,
            _ => actualVersion >= dependency.Version
        };
    }
}