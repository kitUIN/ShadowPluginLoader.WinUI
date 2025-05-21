using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;

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
    public ConcurrentDictionary<string, PluginEntryPoint[]> EntryPoints { get; }

    /// <summary>
    /// DllFiles
    /// </summary>
    public ConcurrentDictionary<string, string> DllFiles { get; }

    /// <summary>
    /// Load SortPluginData From Uri
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <returns></returns>
    public Task<SortPluginData<TMeta>> LoadSortPluginData(Uri uri, string tempFolder);

    /// <summary>
    /// Check if MetaData is valid
    /// </summary>
    /// <param name="meta"></param>
    public void CheckMetaDataValid(TMeta meta);

    /// <summary>
    /// Get Main Plugin
    /// </summary>
    /// <param name="meta"></param>
    public Task<Type> GetMainPluginType(SortPluginData<TMeta> sortPluginData);
}