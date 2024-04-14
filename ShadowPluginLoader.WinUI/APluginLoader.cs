using CustomExtensions.WinUI;
using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace ShadowPluginLoader.WinUI;

public abstract partial class APluginLoader<TMeta, TIMeta, TIPlugin>
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
    /// The Plugin ddl Prefix
    /// <para>
    /// If My Plugin Called `Shadow.Plugin.Example.dll`, `Shadow.Plugin` Is The PluginPrefix
    /// </para>
    /// </summary>
    protected abstract string PluginPrefix { get; }

    /// <summary>
    /// DI Services
    /// </summary>
    protected Container Services { get; }

    /// <summary>
    /// Logger
    /// </summary>
    private ILogger? Logger { get; }

    /// <summary>
    /// Default
    /// </summary>
    /// <param name="logger">log</param>
    /// <param name="services">di services</param>
    protected APluginLoader(ILogger logger, Container services)
    {
        Logger = logger;
        Services = services;
    }

    /// <summary>
    /// All Plugins
    /// </summary>
    private readonly Dictionary<string, TIPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);

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
        var result = new List<string>();
        await GetAllPathAsync(dir, result);
        foreach (var pluginFilePath in result)
        {
            await PreOnePluginAsync(pluginFilePath);
        }
    }

    /// <summary>
    /// Get All Plugin Dll Paths From The Plugin Folder
    /// </summary>
    /// <param name="dir">The Plugin Folder</param>
    /// <param name="result">All Plugin Dll Paths</param>
    protected async Task GetAllPathAsync(DirectoryInfo dir, IList<string> result)
    {
        var pls = dir.GetFiles(PluginPrefix + ".*.dll");
        if (pls.Length > 0)
        {
            // Only One Plugin Dll In One Plugin Self Folder
            result.Add(pls[0].FullName);
        }

        foreach (var item in dir.GetDirectories())
        {
            await GetAllPathAsync(item, result);
        }
    }

    /// <summary>
    /// Pre-operation For Loading Plugin
    /// </summary>
    /// <param name="pluginFilePath">The Plugin Dll Path</param>
    /// <exception cref="PluginImportError">Not Found Folder Or `plugin.json`</exception>
    protected async Task PreOnePluginAsync(string pluginFilePath)
    {
        var dirPath = Path.GetDirectoryName(pluginFilePath);
        if (dirPath is null || !Directory.Exists(dirPath))
        {
            // The Folder Containing The Plugin Dll Not Found
            throw new PluginImportError($"Dir Not Found: {dirPath}");
        }

        var json = Path.Combine(dirPath, PluginJson);
        if (!File.Exists(json))
        {
            throw new PluginImportError($"Not Found {PluginJson} In {dirPath}");
        }

        // Load Json From plugin.json
        var meta = JsonSerializer.Deserialize<TMeta>(await File.ReadAllTextAsync(json));
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
        var isEnabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
        if (isEnabled) instance.Enable();
        else instance.Disable();
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
    protected abstract void CheckPluginMetaData(TIMeta meta);

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
    protected TIMeta GetAndCheckPluginMetaData(Type plugin)
    {
        var meta = plugin.GetPluginMetaData<TIMeta>();
        if (meta is null) throw new PluginImportError($"{plugin.FullName}: MetaData Not Found");
        CheckPluginMetaData(meta);
        return meta;
    }

    /// <summary>
    /// Check PluginMetaData (Async From Path)
    /// </summary>
    /// <param name="meta">PluginMetaData</param>
    /// <param name="path">Plugin Dll Path</param>
    /// <exception cref="PluginImportError">PluginMetaData Is Null</exception>
    protected async Task CheckPluginMetaDataAsync(TIMeta meta, string path)
    {
        if (meta is null) throw new PluginImportError($"MetaData Not Found: {path}");
        CheckPluginMetaData(meta);
        var sortData = new SortPluginData(meta.Id, meta.Requires);
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
        var t = asm.ForeignAssembly.GetExportedTypes().FirstOrDefault(x => x.IsAssignableTo(typeof(TIPlugin)));
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
    protected TIPlugin RegisterPluginMain(Type plugin, TIMeta meta)
    {
        Services.Register(typeof(TIPlugin), plugin, Reuse.Singleton);
        var instance = Services.ResolveMany<TIPlugin>()
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
    protected abstract void LoadPluginDi(Type plugin, TIMeta meta);

}