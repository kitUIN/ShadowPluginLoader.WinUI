using NuGet.Versioning;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// 
/// </summary>
public class DependencyChecker<TMeta> : IDependencyChecker<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <inheritdoc />
    public ConcurrentDictionary<string, NuGetVersion> LoadedPlugins { get; } = new();

    /// <inheritdoc />
    public ConcurrentDictionary<string, TMeta> LoadedMetas { get; } = new();

    /// <inheritdoc />
    public async Task CheckUpgrade(string id, Uri uri)
    {
        var meta = await MetaDataHelper.ToMetaAsyncFromZip<TMeta>(uri.LocalPath);
        var current = LoadedMetas[id];
        if (current.Version >= meta.Version)
            throw new PluginUpgradeException(
                $"{id} Plugin Current: {current.Version}, Upgrade: {meta.Version}");
        foreach (var dependency in meta.Dependencies)
        {
            if (!LoadedPlugins.TryGetValue(dependency.Id, out var loadedPluginVersion))
                throw new PluginDependencyException($"Dependency Not Found: {dependency.Id}");

            if (!dependency.Need.Satisfies(loadedPluginVersion))
            {
                throw new PluginDependencyException(
                    $"Version Not Satisfied: {dependency.Id}, Need: {dependency.Need}, Actual: {loadedPluginVersion}");
            }
        }
    }

    /// <inheritdoc />
    public DependencyCheckResult<TMeta> DetermineLoadOrder(IEnumerable<SortPluginData<TMeta>> plugins)
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
            .Where(p => !LoadedPlugins.ContainsKey(p.MetaData.DllName))
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
    /// <exception cref="PluginDependencyException"></exception>
    private void SortDependencies(SortPluginData<TMeta> plugin, List<SortPluginData<TMeta>> plugins,
        List<SortPluginData<TMeta>> sortedPlugins, HashSet<string> visited)
    {
        visited.Add(plugin.Id);

        foreach (var dependency in plugin.Dependencies)
        {
            var dependentPlugin = plugins.FirstOrDefault(p => p.Id == dependency.Id);

            if (dependentPlugin == null)
            {
                if (!LoadedPlugins.TryGetValue(dependency.Id, out var loadedPluginVersion))
                    throw new PluginDependencyException($"Dependency Not Found: {dependency.Id}");

                if (!dependency.Need.Satisfies(loadedPluginVersion))
                {
                    throw new PluginDependencyException(
                        $"Version Not Satisfied: {dependency.Id}, Need: {dependency.Need}, Actual: {loadedPluginVersion}");
                }

                continue;
            }

            if (!dependency.Need.Satisfies(dependentPlugin.Version))
            {
                throw new PluginDependencyException(
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
}