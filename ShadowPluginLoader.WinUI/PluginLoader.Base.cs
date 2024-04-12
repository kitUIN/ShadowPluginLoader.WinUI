using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI;

public abstract partial class PluginLoader<M, IM, IPT> : IPluginLoader<IPT>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ImportOne<T>() where T : IPT
    {
        LoadPlugin(typeof(T));
    }
    public async Task ImportAllAsync(string directoryPath)
    {
        await ImportOneAsync(directoryPath);
    }

    /// <summary>
    /// 从路径导入
    /// </summary>
    public async Task ImportOneAsync(string pluginPath)
    {
        tempSortPlugins.Clear();
        sortLoader.Clear();
        var dir = new DirectoryInfo(pluginPath);
        if (dir is null) return;
        await GetPathFromPluginsPathAsync(dir);
        var sorted = PluginSortExtension.SortPlugin(sortLoader, x => x.Requires, tempSortPlugins);
        LoadPlugins(sorted);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void PluginEnabled(string id)
    {
        if (plugins.ContainsKey(id))
        {
            plugins[id].Enable();

            Logger?.Information("{Pre}{Id}: Enabled",
                LoggerPrefix, id);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void PluginDisabled(string id)
    {
        if (plugins.ContainsKey(id))
        {
            plugins[id].Disable();
            Logger?.Information("{Pre}{Id}: Disabled",
                LoggerPrefix, id);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsEnabled(string id)
    {
        if (plugins.ContainsKey(id))
        {
            return plugins[id].IsEnabled;
        }
        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Delete(string id)
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

}