using System;
using System.IO;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;

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
    public Task<FileInfo> ScanAsync(Uri uri, string tempFolder, string targetFolder);

    /// <summary>
    /// Upgrade
    /// </summary>
    public Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder);

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string pluginId);
}