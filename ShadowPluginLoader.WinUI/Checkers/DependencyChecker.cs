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
    public List<SortPluginData<TMeta>> DetermineLoadOrder(List<SortPluginData<TMeta>> plugins)
    {
        var sortedPlugins = new List<SortPluginData<TMeta>>();
        var visited = new HashSet<string>();

        foreach (var plugin in plugins.OrderBy(p => p.Priority))
        {
            if (!visited.Contains(plugin.Id))
            {
                SortDependencies(plugin, plugins, sortedPlugins, visited);
            }
        }

        return sortedPlugins;
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
                throw new PluginImportException($"Dependency Not Found: {dependency.Id}");
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
    private bool IsVersionSatisfied(string actualVersion, PluginDependency dependency)
    {
        return dependency.Comparer switch
        {
            PluginDependencyComparer.Lesser => new Version(actualVersion) <= dependency.Version,
            PluginDependencyComparer.Same => new Version(actualVersion) == dependency.Version,
            _ => new Version(actualVersion) >= dependency.Version
        };
    }
}