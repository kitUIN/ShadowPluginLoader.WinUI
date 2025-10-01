using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Scanners;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// <inheritdoc />
/// </summary>
public abstract partial class AbstractPluginLoader<TMeta, TAPlugin> : IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <inheritdoc />
    public virtual IPluginScanner<TAPlugin, TMeta> PluginScanner { get; }

    /// <summary>
    /// DependencyChecker
    /// </summary>
    protected IDependencyChecker<TMeta> DependencyChecker { get; } = new DependencyChecker<TMeta>();

    /// <summary>
    /// UpgradeChecker
    /// </summary>
    protected IUpgradeChecker UpgradeChecker { get; } = new UpgradeChecker();

    /// <summary>
    /// RemoveChecker
    /// </summary>
    protected IRemoveChecker RemoveChecker { get; } = new RemoveChecker();

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public bool? IsEnabled(string id)
    {
        if (_plugins.TryGetValue(id, out var plugin))
        {
            return plugin.IsEnabled;
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IList<TAPlugin> GetPlugins()
    {
        return _plugins.Values.ToList();
    }


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public TAPlugin? GetPlugin(string id)
    {
        return _plugins.GetValueOrDefault(id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IList<TAPlugin> GetPlugins(bool isEnabled)
    {
        return _plugins
            .Where(plugin => plugin.Value.IsEnabled == isEnabled)
            .Select(plugin => plugin.Value)
            .ToList();
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public TAPlugin? GetPlugin(string id, bool isEnabled)
    {
        var plugin = GetPlugin(id);
        return plugin?.IsEnabled == isEnabled ? plugin : null;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void EnablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = true;
        Logger.Information("{Pre}{Id}: Enabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void DisablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = false;
        Logger.Information("{Pre}{Id}: Disabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public Task RemovePlugin(string id)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginRemoveException($"{id} Plugin Not Found");
        var path = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (path == null) throw new PluginRemoveException($"{id} Plugin Path Not Found");
        RemoveChecker.PlanRemove(id, path);
        return Task.CompletedTask;
    }


    /// <inheritdoc />
    public async Task UpgradePlugin(string id, Uri uri)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginUpgradeException($"{id} Plugin not found");
        await GetPluginInstaller(PluginSettingsHelper.GetPluginInstaller(id))
            .PreUpgradeAsync(plugin, uri, TempFolder, PluginFolder);
    }

    /// <inheritdoc />
    public void LoadAsync(IEnumerable<string> pluginIds)
    {
        foreach (var pluginId in pluginIds)
        {
            DependencyChecker.LoadedMetas.TryGetValue(pluginId, out var meta);
            if (meta == null) continue;
            LoadPlugin(meta);
        }
    }
}