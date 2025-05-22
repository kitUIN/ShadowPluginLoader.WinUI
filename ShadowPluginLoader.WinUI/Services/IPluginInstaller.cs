using DryIoc;
using Serilog.Core;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Plugin Installer
/// </summary>
public interface IPluginInstaller
{
    /// <summary>
    /// Installer Priority
    /// Determines the execution order of the installer. 
    /// Installers with lower values are executed earlier.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Identify
    /// </summary>
    public string Identify { get; }

    /// <summary>
    /// Check
    /// </summary>
    /// <returns></returns>
    public bool Check(Uri path);

    /// <summary>
    /// Second Scan
    /// </summary>
    public Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData, string tempFolder,
        string pluginFolder)
        where TMeta : AbstractPluginMetaData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plugin"></param>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <param name="targetFolder"></param>
    /// <returns></returns>
    public Task PreUpgradeAsync<TMeta>(AbstractPlugin<TMeta> plugin, Uri uri,
        string tempFolder, string targetFolder)
        where TMeta : AbstractPluginMetaData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginId"></param>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    public Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plugin"></param> 
    /// <param name="tempFolder"></param>
    /// <param name="targetFolder"></param>
    /// <returns></returns>
    public Task PreRemoveAsync<TMeta>(AbstractPlugin<TMeta> plugin,
        string tempFolder, string targetFolder)
        where TMeta : AbstractPluginMetaData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginId"></param> 
    /// <param name="tempFolder"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    public Task RemoveAsync(string pluginId, string tempFolder, string targetPath);

    /// <summary>
    /// EntryPoints
    /// </summary>
    public ConcurrentDictionary<string, PluginEntryPoint[]> EntryPoints { get; }

    /// <summary>
    /// LoadAsync SortPluginData From Uri
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <returns></returns>
    public Task<SortPluginData<TMeta>> LoadSortPluginData<TMeta>(Uri uri, string tempFolder)
        where TMeta : AbstractPluginMetaData;


    /// <summary>
    /// Get Main Plugin
    /// </summary>
    /// <param name="sortPluginData"></param>
    public Task<Type> GetMainPluginType<TMeta>(SortPluginData<TMeta> sortPluginData)
        where TMeta : AbstractPluginMetaData;
}