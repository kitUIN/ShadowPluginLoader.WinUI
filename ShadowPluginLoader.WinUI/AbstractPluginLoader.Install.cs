using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// PluginInstaller
    /// </summary>
    [Autowired]
    protected IPluginInstaller<TMeta> PluginInstaller { get; }

    /// <summary>
    /// RemoveChecker
    /// </summary>
    [Autowired]
    protected IRemoveChecker RemoveChecker { get; }

    /// <inheritdoc />
    public async Task InstallAsync(IEnumerable<string> shadowFiles, IProgress<InstallProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = await PluginInstaller.InstallAsync(shadowFiles, progress, cancellationToken);
        var session = PluginScanner.StartScan();
        foreach (var data in result)
        {
            session.Scan(new Uri(Path.Combine(BaseSdkConfig.PluginFolderPath,
                data.MetaData.DllName, data.MetaData.DllName, "plugin.json")));
        }

        var plugins = await session.FinishAsync();

        progress?.Report(new InstallProgress("", 95, InstallProgressStep.Loading));
        Load(plugins);
        progress?.Report(new InstallProgress("", 100, InstallProgressStep.Success));
    }


    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await RemoveChecker.CheckRemoveAsync();
        await PluginInstaller.CheckUpgradeAsync();
    }


    /// <inheritdoc />
    public async Task UpgradePlugin(string id, Uri uri)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginUpgradeException($"{id} Plugin not found");
        var dir = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (dir == null) throw new PluginUpgradeException($"{id} Plugin Path Not Found");
        if (!(uri.IsFile && uri.LocalPath.EndsWith(".sdow")))
            throw new PluginUpgradeException($"{id} Plugin Upgrade Uri {uri} not support");
        await DependencyChecker.CheckUpgrade(id, uri);
        PluginInstaller.PlanUpgrade(id, dir, uri.LocalPath);
        PluginEventService.InvokePluginPlanRemove(this, new PluginEventArgs(id, PluginStatus.PlanRemove));
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public Task RemovePlugin(string id)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginRemoveException($"{id} Plugin Not Found");
        var dir = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (dir == null) throw new PluginRemoveException($"{id} Plugin Path Not Found");
        RemoveChecker.PlanRemove(id, dir);
        PluginEventService.InvokePluginPlanRemove(this, new PluginEventArgs(id, PluginStatus.PlanRemove));
        return Task.CompletedTask;
    }
}