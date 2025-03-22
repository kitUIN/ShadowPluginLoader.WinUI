using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// MetaData Checker
/// </summary>
/// <typeparam name="TMeta">MetaData Type</typeparam>
public interface IMetaDataChecker<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// EntryPoints
    /// </summary>
    public Dictionary<string, JsonNode?> EntryPoints { get; }

    /// <summary>
    /// DllFiles
    /// </summary>
    public Dictionary<string, string> DllFiles { get; }

    /// <summary>
    /// Load MetaData From plugin.json
    /// </summary>
    /// <param name="pluginJson"></param>
    /// <returns></returns>
    public TMeta LoadMetaData(FileInfo pluginJson);

    /// <summary>
    /// Load MetaData From Zip
    /// </summary>
    /// <param name="zip"></param>
    /// <returns></returns>
    public TMeta LoadMetaData(string zip);

    /// <summary>
    /// Check if MetaData is valid
    /// </summary>
    /// <param name="meta"></param>
    public void CheckMetaDataValid(TMeta meta);

    /// <summary>
    /// Get Main Plugin
    /// </summary>
    /// <param name="meta"></param>
    public Task<Type> GetMainPluginType(TMeta meta);
}