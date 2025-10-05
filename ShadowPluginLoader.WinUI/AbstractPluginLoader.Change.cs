using System.IO;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// UpgradeChecker
    /// </summary>
    protected IUpgradeChecker UpgradeChecker { get; }

    /// <summary>
    /// RemoveChecker
    /// </summary>
    protected IRemoveChecker RemoveChecker { get; }

    /// <inheritdoc />
    public async Task CheckUpgradeAndRemoveAsync()
    {
        await RemoveChecker.CheckRemoveAsync();
        await UpgradeChecker.CheckUpgradeAsync();
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