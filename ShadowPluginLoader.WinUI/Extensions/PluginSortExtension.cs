using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
namespace ShadowPluginLoader.WinUI.Extensions;

/// <summary>
/// PluginExtension That Can Sort Plugins
/// </summary>
public static class PluginSortExtension
{
    /// <summary>
    /// SortPlugin
    /// </summary>
    /// <param name="source">SortPluginData List</param>
    /// <param name="getDependencies">Get Dependencies Func</param>
    /// <param name="sortData">sortData Dict</param>
    /// <typeparam name="T">SortPluginData</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> SortPlugin<T>(IEnumerable<T> source, Func<T, string[]?> getDependencies,Dictionary<string, T> sortData) where T : SortPluginData
    {
        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>();

        foreach (var item in source)
        {
            Visit(item, getDependencies, sorted, visited, sortData);
        }

        return sorted;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="getDependencies"></param>
    /// <param name="sorted"></param>
    /// <param name="visited"></param>
    /// <param name="sortData"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentException"></exception>
    private static void Visit<T>(T item, Func<T, string[]?> getDependencies, ICollection<T> sorted, IDictionary<T, bool> visited, Dictionary<string, T> sortData) where T : SortPluginData
    {
        var alreadyVisited = visited.TryGetValue(item, out var inProcess);

        // If the vertex has already been visited, it is returned directly as
        if (alreadyVisited)
        {
            // If the current node is processed, then a circular reference exists
            if (inProcess)
            {
                throw new ArgumentException("Cyclic dependency found.");
            }
        }
        else
        {
            // visit top 
            visited[item] = true;
            // get all dependencies
            var dependencies = getDependencies(item);
            // If the set of dependencies is not empty, traverse to access its dependency nodes
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    if (sortData.TryGetValue(dependency, out var value))
                    {
                        Visit(value, getDependencies, sorted, visited, sortData);
                    }
                }
            }
            // finish <- false
            visited[item] = false;
            sorted.Add(item);
        }
    }

}
