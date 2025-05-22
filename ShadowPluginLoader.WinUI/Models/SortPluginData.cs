using System;
using System.Collections.Generic;
using System.IO;

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
    /// Determines the execution order of the plugin. 
    /// Plugins with lower values are executed earlier.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Version
    /// </summary>
    public Version Version { get; }


    /// <summary>
    /// MetaData
    /// </summary>
    public TMeta MetaData { get; }

    /// <summary>
    /// Path
    /// </summary>
    public string Path { get; } = null!;

    /// <summary>
    /// Link
    /// </summary>
    public Uri Link { get; } = null!;

    /// <summary>
    /// SortPluginData
    /// </summary>
    private SortPluginData(TMeta metaData)
    {
        MetaData = metaData;
        Id = metaData.Id;
        Priority = metaData.Priority;
        Version = new Version(metaData.Version);
        foreach (var dep in metaData.Dependencies)
        {
            Dependencies.Add(dep);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaData"></param>
    /// <param name="uri"></param>
    public SortPluginData(TMeta metaData, Uri uri) : this(metaData)
    {
        Path = uri.AbsolutePath;
        Link = uri;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaData"></param>
    /// <param name="path"></param>
    public SortPluginData(TMeta metaData, string path) : this(metaData)
    {
        Path = path;
        Link = new Uri(path);
    }
}