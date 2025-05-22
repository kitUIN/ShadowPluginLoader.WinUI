using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Base Plugin Installer
/// </summary>
public partial class BasePluginInstaller : IPluginInstaller
{
    /// <summary>
    /// Logger
    /// </summary>
    [Autowired]
    public ILogger Logger { get; }

    /// <inheritdoc />
    public virtual int Priority => 100;

    /// <inheritdoc />
    public virtual bool Check(Uri path)
    {
        return true;
    }

    /// <inheritdoc />
    public virtual Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData,
        string tempFolder, string pluginFolder) where TMeta : AbstractPluginMetaData
    {
        return Task.FromResult(sortPluginData);
    }

    /// <inheritdoc />
    public virtual Task<string?> PreUpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder)
    {
        return Task.FromResult(uri.LocalPath)!;
    }


    /// <inheritdoc />
    public virtual Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder)
    {
        return Task.CompletedTask;
    }

}