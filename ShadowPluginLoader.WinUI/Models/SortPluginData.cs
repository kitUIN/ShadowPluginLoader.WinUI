using System;
using System.Collections.Generic;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// SortPluginData
/// </summary>
public class SortPluginData<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// Plugin Dependencies
    /// </summary>
    public List<PluginDependency> Dependencies { get; } = [];

    /// <summary>
    /// Plugin Id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Plugin Priority
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Version
    /// </summary>
    public string Version { get; }


    /// <summary>
    /// MetaData
    /// </summary>
    public TMeta MetaData { get; }

    /// <summary>
    /// SortPluginData
    /// </summary>
    public SortPluginData(TMeta metaData)
    {
        MetaData = metaData;
        Id = metaData.Id;
        Priority = metaData.Priority;
        Version = metaData.Version;
        foreach (var dep in metaData.Dependencies)
        {
            var deps = dep.Split("=", 2);
            if (deps.Length == 1)
            {
                Dependencies.Add(new PluginDependency(deps[0]));
            }
            else if (deps.Length == 2)
            {
                Dependencies.Add(new PluginDependency(deps[0], deps[1]));
            }
        }
    }
}