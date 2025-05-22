using Microsoft.UI.Xaml.Shapes;
using Serilog;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.IO;
using System.Threading.Tasks;

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
    public virtual string Identify => "Base";

    /// <inheritdoc />
    public virtual bool Check(Uri path)
    {
        return true;
    }

    /// <inheritdoc />
    public virtual Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData,
        string tempFolder, string pluginFolder) where TMeta : AbstractPluginMetaData
    {
        PluginSettingsHelper.SetPluginInstaller(sortPluginData.Id, Identify);
        return Task.FromResult(sortPluginData);
    }

    /// <inheritdoc />
    public virtual Task PreUpgradeAsync<TMeta>(AbstractPlugin<TMeta> plugin, Uri uri, string tempFolder,
        string targetFolder) where TMeta : AbstractPluginMetaData
    {
        return Task.FromResult(uri.LocalPath)!;
    }


    /// <inheritdoc />
    public virtual Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath)
    {
        PluginSettingsHelper.DeleteUpgradePluginPath(pluginId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task PreRemoveAsync<TMeta>(AbstractPlugin<TMeta> plugin, string tempFolder, string targetFolder)
        where TMeta : AbstractPluginMetaData
    {
        plugin.PlanRemove = true;
        PluginSettingsHelper.SetPluginPlanRemove(plugin.Id, targetFolder);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task RemoveAsync(string pluginId, string tempFolder, string targetPath)
    {
        if (Directory.Exists(targetPath)) Directory.Delete(targetPath, true);
        PluginSettingsHelper.DeleteRemovePluginPath(pluginId);
        return Task.CompletedTask;
    }
}