using Serilog.Core;
using ShadowPluginLoader.WinUI.Models;
using System;
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
    /// <param name="pluginId"></param>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <param name="targetFolder"></param>
    /// <returns></returns>
    public Task<string?> PreUpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginId"></param>
    /// <param name="uri"></param>
    /// <param name="tempFolder"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    public Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath);

}