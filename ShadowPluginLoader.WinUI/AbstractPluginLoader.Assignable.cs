﻿using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract PluginLoader
/// </summary>
/// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="AbstractPluginMetaData"/></typeparam>
/// <typeparam name="TAPlugin">Your Custom Interface IPlugin Assignable To <see cref="AbstractPlugin"/></typeparam>
public abstract partial class AbstractPluginLoader<TMeta, TAPlugin> : IPluginLoader<TAPlugin>
    where TMeta : AbstractPluginMetaData
    where TAPlugin : AbstractPlugin
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Import(Type type)
    {
        try
        {
            if (!type.IsAssignableTo(typeof(TAPlugin)))
                throw new PluginImportException($"{type.FullName} is not assignable to {typeof(TAPlugin).FullName}");
            LoadPlugin(type);
        }
        catch (PluginImportException e)
        {
            Logger.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Import<TPlugin>()
    {
        Import(typeof(TPlugin));
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
            zipPath = await DownloadZipFromPath(zipPath);
            var meta = await CheckPluginInZip(zipPath);
            var outPath = Path.Combine(PluginFolder, meta!.DllName);
            Logger.Debug("Plugin OutPath: {t}", outPath);
            await ImportFromDirAsync(await UnZip(zipPath, outPath));
        }
        catch (PluginImportException e)
        {
            Logger.Warning("{Pre}{Message}", LoggerPrefix, e.Message);
        }
    }

    /// <summary>
    /// Check Any Plugin Plan To Upgrade
    /// </summary>
    protected async Task CheckUpgrade()
    {
        var settings = PluginSettingsHelper.GetPluginUpgradePaths();
        foreach (var setting in settings)
        {
            var zipPath = (string)setting.Value;
            var meta = await CheckPluginInZip(zipPath);
            var outPath = Path.Combine(PluginFolder, meta!.DllName);
            await UnZip(zipPath, outPath);
            PluginSettingsHelper.DeleteUpgradePluginPath(meta.Id);
        }
    }

    /// <summary>
    /// Check Any Plugin Plan To Remove
    /// </summary>
    protected void CheckRemove()
    {
        var settings = PluginSettingsHelper.GetPluginRemovePaths();
        foreach (var setting in settings)
        {
            var path = (string)setting.Value;
            if (Directory.Exists(path)) Directory.Delete(path, true);
            PluginSettingsHelper.DeleteRemovePluginPath(setting.Key);
            PluginEventService.InvokePluginRemoved(this,
                new Args.PluginEventArgs(setting.Key, Enums.PluginStatus.Removed));
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task ImportFromDirAsync(string pluginPath)
    {
        try
        {
            CheckRemove();
            await CheckUpgrade();
            _tempSortPlugins.Clear();
            _sortLoader.Clear();
            await CheckPluginMetaDataFromJson(new DirectoryInfo(pluginPath));
            var sorted = PluginSortExtension.SortPlugin(_sortLoader, x => x.Dependencies, _tempSortPlugins);
            LoadPluginType(sorted);
        }
        catch (PluginImportException e)
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
        return _plugins.GetValueOrDefault(id);
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
        return null;
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
    public void RemovePlugin(string id)
    {
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginRemoveException($"{id} Plugin Not Found");
        var path = Path.GetDirectoryName(plugin.GetType().Assembly.Location);
        if (path == null) throw new PluginRemoveException($"{id} Plugin Path Not Found");
        plugin.PlanRemove = true;
        PluginSettingsHelper.SetPluginPlanRemove(id, path);
    }

    /// <summary>
    /// If local uri , return local uri. If http uri , download and return local uri
    /// </summary>
    /// <param name="newVersionZip"></param>
    /// <returns></returns>
    protected async Task<string> DownloadZipFromPath(string newVersionZip)
    {
        if (!newVersionZip.StartsWith("http")) return newVersionZip;
        var fileName = Path.GetFileName(newVersionZip);
        var destinationPath = Path.Combine(TempFolder, fileName);
        if (!Directory.Exists(TempFolder)) Directory.CreateDirectory(TempFolder);
        Logger.Information("Start To Download File {httpPath} To {destinationPath}",
            newVersionZip, destinationPath);
        using var client = new HttpClient();
        using var response = await client.GetAsync(new Uri(newVersionZip), HttpCompletionOption.ResponseHeadersRead);
        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
        Logger.Information("Download File {httpPath} To {destinationPath} Success",
            newVersionZip, destinationPath);
        return destinationPath;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meta"></param>
    /// <param name="pluginMetaData"></param>
    /// <exception cref="PluginUpgradeException"></exception>
    protected void CheckNewVersionMeta(TMeta meta, TMeta pluginMetaData)
    {
        if (new Version(meta.Version) <= new Version(pluginMetaData.Version))
        {
            throw new PluginUpgradeException($"The upgraded version ({meta.Version}) " +
                                             $"should be larger than the current " +
                                             $"version ({pluginMetaData.Version})");
        }
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public async Task UpgradePlugin(string id, string newVersionZip)
    {
        newVersionZip = await DownloadZipFromPath(newVersionZip);
        var plugin = GetPlugin(id);
        if (plugin == null) throw new PluginUpgradeException($"{id} Plugin not found");
        var meta = await CheckPluginInZip(newVersionZip);
        if (meta is null) throw new PluginUpgradeException($"Not Found `plugin.json` File In {newVersionZip}");
        var pluginMetaData = plugin.GetType().GetPluginMetaData<TMeta>()
                             ?? throw new PluginUpgradeException($"{plugin.Id}: MetaData Not Found");
        CheckNewVersionMeta(meta, pluginMetaData);

        plugin.PlanUpgrade = true;
        PluginSettingsHelper.SetPluginUpgradePath(id, newVersionZip);
    }
}