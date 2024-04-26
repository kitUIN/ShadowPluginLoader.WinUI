﻿using CustomExtensions.WinUI;
using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace ShadowPluginLoader.WinUI;

public abstract partial class APluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// Logger Print With Prefix
    /// </summary>
    protected string LoggerPrefix => "[PluginLoader] ";

    /// <summary>
    /// Plugin MetaData Json File Name
    /// </summary>
    protected string PluginJson => "plugin.json";
    /// <summary>
    /// Plugins Folder
    /// </summary>
    protected string PluginFolder => "plugins";

    /// <summary>
    /// DI Services
    /// </summary>
    public static Container Services { get; set; }

    /// <summary>
    /// Logger
    /// </summary>
    private ILogger? Logger { get; }

    /// <summary>
    /// Default
    /// </summary>
    /// <param name="logger">log</param>
    protected APluginLoader(ILogger logger)
    {
        Logger = logger;
    }
    /// <summary>
    /// Default
    /// </summary>
    protected APluginLoader():
        this(Log.ForContext<APluginLoader<TMeta, TAPlugin>>())
    {
    }
    /// <summary>
    /// All Plugins
    /// </summary>
    private readonly Dictionary<string, TAPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Temp Sort
    /// </summary>
    private readonly Dictionary<string, SortPluginData> _tempSortPlugins = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Sort Loader
    /// </summary>
    private readonly List<SortPluginData> _sortLoader = new();


    /// <summary>
    /// Check PluginMetaData From Json
    /// </summary>
    /// <param name="dir">Plugin Dir</param>
    protected async Task CheckPluginMetaDataFromJson(DirectoryInfo dir)
    {
        var result = GetAllPathAsync(dir);
        foreach (var pluginFilePath in result)
        {
            await PreOnePluginAsync(pluginFilePath);
        }
    }

    /// <summary>
    /// Get All Plugin JSON Paths From The Plugin Folder
    /// </summary>
    /// <param name="dir">The Plugin Folder</param>
    protected List<string> GetAllPathAsync(DirectoryInfo dir)
    {
        var pls = dir.GetFiles(PluginJson,SearchOption.AllDirectories);
        return pls.Select(x => x.FullName).ToList();
    }

    /// <summary>
    /// Pre-operation For Loading Plugin
    /// </summary>
    /// <param name="pluginJsonFilePath">The Plugin Json Path</param>
    /// <exception cref="PluginImportError">Not Found Dll Or folder Or `plugin.json`</exception>
    protected async Task PreOnePluginAsync(string pluginJsonFilePath)
    {
        if (!File.Exists(pluginJsonFilePath)) throw new PluginImportError($"Not Found {pluginJsonFilePath}");
        // Load Json From plugin.json
        var meta = JsonSerializer.Deserialize<TMeta>(await File.ReadAllTextAsync(pluginJsonFilePath));
        var dirPath = Path.GetDirectoryName(pluginJsonFilePath);
        
        if (dirPath is null || !Directory.Exists(dirPath))
        {
            // The Folder Containing The Plugin Dll Not Found
            throw new PluginImportError($"Dir Not Found: {dirPath}");
        }
        var pluginFilePath = Path.Combine(dirPath, meta!.Id + ".dll");
        if (!File.Exists(pluginFilePath)) throw new PluginImportError($"Not Found {pluginFilePath}");
        await CheckPluginMetaDataAsync(meta!, pluginFilePath);
    }


    /// <summary>
    /// Load Plugin From Type
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    protected void LoadPlugin(Type? plugin)
    {
        CheckPluginType(plugin);
        var meta = GetAndCheckPluginMetaData(plugin!);
        var instance = RegisterPluginMain(plugin!, meta);
        LoadPluginDi(plugin!, meta);
        _plugins[meta.Id] = instance;
        instance.IsEnabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
        Logger?.Information("{Pre}{ID}({Name}): Load Success!",
            LoggerPrefix, meta.Id, meta.Name);
    }


    /// <summary>
    /// Check Plugin Type Not Null
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <exception cref="PluginImportError">Plugin Type Is Null</exception>
    protected void CheckPluginType(Type? plugin)
    {
        if (plugin is null) throw new PluginImportError("Plugin Type Not Found");
    }

    /// <summary>
    /// Check PluginMetaData(Default: No Check)
    /// </summary>
    /// <param name="meta">PluginMetaData</param>
    protected virtual void CheckPluginMetaData(TMeta meta)
    {

    }
/// <summary>
/// LoadPlugin From SortedPluginTypes
/// </summary>
/// <param name="sortPlugins">SortedPluginData</param>
protected void LoadPluginType(IEnumerable<SortPluginData> sortPlugins)
    {
        foreach (var data in sortPlugins)
        {
            LoadPlugin(data.PluginType);
        }
    }

    /// <summary>
    /// Get And Check PluginMetaData
    /// </summary>
    /// <param name="plugin">PluginMetaData</param>
    /// <returns>PluginMetaData</returns>
    /// <exception cref="PluginImportError">PluginMetaData Type Is Null</exception>
    protected TMeta GetAndCheckPluginMetaData(Type plugin)
    {
        var meta = plugin.GetPluginMetaData<TMeta>() 
            ?? throw new PluginImportError($"{plugin.FullName}: MetaData Not Found");
        CheckPluginMetaData(meta);
        return meta;
    }

    /// <summary>
    /// Check PluginMetaData (Async From Path)
    /// </summary>
    /// <param name="meta">PluginMetaData</param>
    /// <param name="path">Plugin Dll Path</param>
    /// <exception cref="PluginImportError">PluginMetaData Is Null</exception>
    protected async Task CheckPluginMetaDataAsync(TMeta meta, string path)
    {
        if (meta is null) throw new PluginImportError($"MetaData Not Found: {path}");
        CheckPluginMetaData(meta);
        var sortData = new SortPluginData(meta.Id, meta.Dependencies);
        if (_tempSortPlugins.ContainsKey(meta.Id) || _plugins.ContainsKey(meta.Id))
        {
            // If Loaded, Next One
            Logger?.Warning("{Pre}{ID}({Name}): Exists, Continue",
                LoggerPrefix, meta.Id, meta.Name);
            return;
        }

        // Load Asm From Dll
        var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(path);
        // Try Get First Exported Type AssignableTo TIPlugin
        var t = asm.ForeignAssembly.GetExportedTypes().FirstOrDefault(x => x.IsAssignableTo(typeof(TAPlugin)));
        CheckPluginType(t);
        sortData.PluginType = t;
        _sortLoader.Add(sortData);
        _tempSortPlugins[sortData.Id] = sortData;
    }

    /// <summary>
    /// Register Plugin Main
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <param name="meta">PluginMetaData</param>
    /// <returns>Plugin Instance</returns>
    /// <exception cref="PluginImportError">Can't Register Plugin</exception>
    protected TAPlugin RegisterPluginMain(Type plugin, TMeta meta)
    {
        Services.Register(typeof(TAPlugin), plugin, Reuse.Singleton);
        var instance = Services.ResolveMany<TAPlugin>()
            .FirstOrDefault(x => meta.Id == x.GetId());
        if (instance is null) throw new PluginImportError($"{plugin.Name}: Can't Load Plugin");
        Logger?.Information("插件[{ID}]加载主类成功", meta.Id);
        return instance;
    }

    /// <summary>
    /// Register Plugin DI
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <param name="meta">PluginMetaData</param>
    protected virtual void LoadPluginDi(Type plugin, TMeta meta)
    {

    }

    /// <summary>
    /// Check plugin.json In Zip
    /// </summary>
    /// <param name="zipPath">plugin zip path</param>
    /// <exception cref="PluginImportError">Not Found plugin.json in zip</exception>
    protected void CheckPluginInZip(string zipPath)
    {
        using FileStream zipToOpen = new(zipPath, FileMode.Open);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);
        var jsonEntry = archive.GetEntry(PluginJson) ?? throw new PluginImportError($"Not Found {PluginJson} in zip {zipPath}");
    }

    /// <summary>
    /// UnZip
    /// </summary>
    protected string UnZip(string zipPath,string outputPath)
    {
        ZipFile.ExtractToDirectory(zipPath,outputPath);
        return outputPath;
    }
}