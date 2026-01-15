using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// RemoveChecker
    /// </summary>
    [Autowired]
    protected IRemoveChecker RemoveChecker { get; }

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected IUpgradeChecker UpgradeChecker { get; }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await RemoveChecker.CheckRemoveAsync();
        await UpgradeChecker.CheckUpgradeAsync();
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
        UpgradeChecker.PlanUpgrade(id, dir, uri.LocalPath);
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