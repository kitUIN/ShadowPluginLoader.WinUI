using System;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// <inheritdoc />
/// </summary>
public abstract partial class AbstractPluginLoader<TMeta, TAPlugin> : IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// DependencyChecker
    /// </summary>
    [Autowired]
    protected IDependencyChecker<TMeta> DependencyChecker { get; }

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


    /// <inheritdoc />
    public int Load(IEnumerable<string> pluginIds, IProgress<PipelineProgress>? progress = null)
    {
        var count = 0;
        var enumerable = pluginIds as string[] ?? pluginIds.ToArray();
        var total = enumerable.Length;
        foreach (var pluginId in enumerable)
        {
            try
            {
                DependencyChecker.LoadedMetas.TryGetValue(pluginId, out var meta);
                if (meta == null)
                {
                    throw new PluginNotFoundException($"{pluginId}: Can't find plugin meta.");
                }

                LoadPlugin(meta);
                count++;
                var d = count / total;
                progress?.Report(new PipelineProgress(
                    TotalPercentage: 0.66D + d * 0.33D,
                    TotalStatusValue: meta.Id,
                    Step: InstallPipelineStep.Outbounding,
                    SubPercentage: d,
                    SubStatusValue: meta.Name,
                    SubStep: SubInstallPipelineStep.Outbounding
                ));
            }
            catch (Exception e)
            {
                Logger.Warning(e, "{Pre}{PluginId}: Plugin Load Failed!", LoggerPrefix, pluginId);
            }
        }

        return count;
    }
}