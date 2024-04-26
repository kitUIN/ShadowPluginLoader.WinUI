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
/// <typeparam name="TAPlugin">Your Custom Interface IPlugin Assignable To <see cref="APlugin"/></typeparam>
public abstract partial class APluginLoader<TMeta, TAPlugin> : IPluginLoader<TAPlugin>
    where TMeta : AbstractPluginMetaData
    where TAPlugin : APlugin
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Import(Type type)
    {
        try
        {
            if (!type.IsAssignableTo(typeof(TAPlugin)))
                throw new PluginImportError($"{type.FullName} is not assignable to {typeof(TAPlugin).FullName}");
            LoadPlugin(type);
        }
        catch (PluginImportError e)
        {
            Logger?.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Import(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            Import(type);
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task ImportFromZipAsync(string zipPath)
    {
        try
        {
            CheckPluginInZip(zipPath);
            await ImportAsync(UnZip(PluginFolder, zipPath));
        }
        catch (PluginImportError e)
        {
            Logger?.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// <inheritdoc />
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
        return _plugins.TryGetValue(id, out var plugin) ? plugin : default;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IList<TAPlugin> GetEnabledPlugins()
    {
        return _plugins
            .Where(plugin => plugin.Value.IsEnabled)
            .Select(plugin => plugin.Value)
            .ToList();
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public TAPlugin? GetEnabledPlugin(string id)
    {
        if (GetPlugin(id) is { IsEnabled: true } plugin) return plugin;
        return default;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void EnablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = true;
        Logger?.Information("{Pre}{Id}: Enabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void DisablePlugin(string id)
    {
        if (!_plugins.TryGetValue(id, out var plugin)) return;
        plugin.IsEnabled = false;
        Logger?.Information("{Pre}{Id}: Disabled",
            LoggerPrefix, id);
    }

    /// <summary>
    /// <inheritdoc />
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
    /// <inheritdoc />
    /// </summary>
    public void UpgradePlugin(string id)
    {
    }

 
}