using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Msix Plugin Installer
/// </summary>
[CheckAutowired]
public partial class MsixPluginInstaller : ZipPluginInstaller
{
    /// <inheritdoc />
    public override int Priority => 1;

    /// <inheritdoc />
    public override string Identify => "Msix";

    /// <inheritdoc />
    public override bool Check(Uri path)
    {
        return path.AbsolutePath.EndsWith(".msix", StringComparison.OrdinalIgnoreCase);
    }


    /// <inheritdoc />
    public override async Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData,
        string tempFolder, string pluginFolder)
    {
        var packageManager = new PackageManager();
        var deploymentResult = await packageManager.AddPackageAsync(
            sortPluginData.Link,
            null,
            DeploymentOptions.None
        );
        if (deploymentResult is null)
        {
            throw new PluginImportException($"Install Optional Package {sortPluginData.Id} Error");
        }

        if (deploymentResult != null && deploymentResult.ErrorText != null)
        {
            throw new PluginImportException(
                $"Install Optional Package {sortPluginData.Id} Error: {deploymentResult?.ErrorText}");
        }

        var package = packageManager.FindPackage(deploymentResult!.ActivityId.ToString("D"));
        var jsonEntryFile = new DirectoryInfo(package.InstalledPath)
            .GetDirectories("Assets", SearchOption.AllDirectories)
            .Select(assetDir => new FileInfo(Path.Combine(assetDir.FullName, "plugin.json")))
            .FirstOrDefault(file => file.Exists);
        if (jsonEntryFile == null)
            throw new PluginImportException($"Not Found plugin.json in zip {sortPluginData.Path}");
        return await base.InstallAsync(new SortPluginData<TMeta>(sortPluginData.MetaData, jsonEntryFile.FullName),
            tempFolder, pluginFolder);
    }

    /// <inheritdoc />
    public override async Task PreUpgradeAsync<TMeta>(AbstractPlugin<TMeta> plugin, Uri uri, string tempFolder,
        string targetFolder)
    {
        var newVersionUri = await FileHelper.DownloadFileAsync(tempFolder, uri, Logger);
        plugin.PlanUpgrade = true;
        PluginSettingsHelper.SetPluginUpgradePath(plugin.Id, newVersionUri.LocalPath,
            Path.GetDirectoryName(plugin.GetType().Assembly.Location)!);
    }

    /// <inheritdoc />
    public override async Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath)
    {
        var packageManager = new PackageManager();
        var deploymentResult = await packageManager.AddPackageAsync(
            uri,
            null,
            DeploymentOptions.None
        );
        if (deploymentResult is null)
        {
            throw new PluginUpgradeException($"Update Optional Package {pluginId} Error");
        }

        if (deploymentResult != null && deploymentResult.ErrorText != null)
        {
            throw new PluginUpgradeException(
                $"Update Optional Package {pluginId} Error: {deploymentResult?.ErrorText}");
        }

        await base.UpgradeAsync(pluginId, uri, tempFolder, targetPath);
    }

    /// <inheritdoc />
    public override Task RemoveAsync(string pluginId, string tempFolder, string targetPath)
    {
        // var packageManager = new PackageManager();
        // var deploymentResult = await packageManager.RemovePackageAsync(packageFullName);
        //
        // if (deploymentResult is null)
        // {
        //     throw new PluginRemoveException($"Remove Optional Package {pluginId} Error");
        // }
        //
        // if (deploymentResult != null && deploymentResult.ErrorText != null)
        // {
        //     throw new PluginRemoveException(
        //         $"Remove Optional Package {pluginId} Error: {deploymentResult?.ErrorText}");
        // }

        PluginSettingsHelper.DeleteRemovePluginPath(pluginId);
        return Task.CompletedTask;
    }
}