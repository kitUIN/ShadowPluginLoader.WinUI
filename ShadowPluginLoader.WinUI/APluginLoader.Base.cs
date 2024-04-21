using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract PluginLoader
/// </summary>
/// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="AbstractPluginMetaData"/></typeparam>
/// <typeparam name="TIPlugin">Your Custom Interface IPlugin Assignable To <see cref="IPlugin"/></typeparam>
public abstract partial class APluginLoader<TMeta, TIPlugin> : IPluginLoader<TIPlugin>
    where TMeta : AbstractPluginMetaData
    where TIPlugin : IPlugin
{
    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.Import(Type)"/>
    /// </summary>
    public void Import(Type type)
    {
        try
        {
            if (!type.IsAssignableTo(typeof(TIPlugin)))
                throw new PluginImportError($"{type.FullName} is not assignable to {typeof(TIPlugin).FullName}");
            LoadPlugin(type);
        }
        catch (PluginImportError e)
        {
            Logger?.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.Import(IEnumerable{Type})"/>
    /// </summary>
    public void Import(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Import(type);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.ImportAllAsync"/>
    /// </summary>
    public async Task ImportAllAsync(string directoryPath)
    {
        await ImportAsync(directoryPath);
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.ImportAsync"/>
    /// </summary>
    public async Task ImportAsync(string pluginPath)
    {
        try
        {
            _tempSortPlugins.Clear();
            _sortLoader.Clear();
            await CheckPluginMetaDataFromJson(new DirectoryInfo(pluginPath));
            var sorted = PluginSortExtension.SortPlugin(_sortLoader, x => x.Dependencies, _tempSortPlugins);
            LoadPluginType(sorted);
        }
        catch (PluginImportError e)
        {
            Logger?.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.IsEnabled"/>
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
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.GetPlugins"/>
    /// </summary>
    public IList<TIPlugin> GetPlugins()
    {
        return _plugins.Values.ToList();
    }


    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.GetPlugin"/>
    /// </summary>
    public TIPlugin? GetPlugin(string id)
    {
        return _plugins.TryGetValue(id, out var plugin) ? plugin : default;
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.GetEnabledPlugins"/>
    /// </summary>
    public IList<TIPlugin> GetEnabledPlugins()
    {
        return _plugins
            .Where(plugin => plugin.Value.IsEnabled)
            .Select(plugin => plugin.Value)
            .ToList();
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.GetEnabledPlugin"/>
    /// </summary>
    public TIPlugin? GetEnabledPlugin(string id)
    {
        if (GetPlugin(id) is { IsEnabled: true } plugin) return plugin;
        return default;
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.EnablePlugin"/>
    /// </summary>
    public void EnablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.Enable();
        Logger?.Information("{Pre}{Id}: Enabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.DisablePlugin"/>
    /// </summary>
    public void DisablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.Disable();
        Logger?.Information("{Pre}{Id}: Disabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.DeletePlugin"/>
    /// </summary>
    public void DeletePlugin(string id)
    {
        /*        try
                {
                    if (GetPlugin(id) is { } plugin)
                    {
                        var file = await plugin.GetType().Assembly.Location.GetFile();
                        var folder = await file.GetParentAsync();
                        plugin.IsEnabled = false;
                        plugin.PluginDeleting();
                        Instances.Remove(plugin);
                        //ApplicationExtensionHost.Current.
                        await folder.DeleteAsync();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("删除插件错误:{E}", ex);
                }

                return false;*/
    }

    /// <summary>
    /// <inheritdoc cref="IPluginLoader{TIPlugin}.UpgradePlugin"/>
    /// </summary>
    public void UpgradePlugin(string id)
    {
    }
}