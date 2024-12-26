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
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Enums;
using SharpCompress.Archives;
using SharpCompress.IO;
using Path = System.IO.Path;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// Logger Print With Prefix
    /// </summary>
    protected virtual string LoggerPrefix => "[PluginLoader] ";

    /// <summary>
    /// Plugin MetaData Json File Name
    /// </summary>
    protected virtual string PluginJson => "plugin.json";

    /// <summary>
    /// Plugins Folder
    /// </summary>
    protected abstract string PluginFolder { get; }

    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// PluginEventService
    /// </summary>
    protected PluginEventService PluginEventService { get; }

    /// <summary>
    /// Default
    /// </summary>
    /// <param name="logger">log</param>
    /// <param name="pluginEventService">pluginEventService</param>
    protected AbstractPluginLoader(ILogger logger, PluginEventService pluginEventService)
    {
        Logger = logger;
        PluginEventService = pluginEventService;
    }

    /// <summary>
    /// Default
    /// </summary>
    protected AbstractPluginLoader(PluginEventService pluginEventService) :
        this(Log.ForContext<AbstractPluginLoader<TMeta, TAPlugin>>(), pluginEventService)
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
    protected virtual async Task CheckPluginMetaDataFromJson(DirectoryInfo dir)
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
    protected virtual List<string> GetAllPathAsync(DirectoryInfo dir)
    {
        var pls = dir.GetFiles(PluginJson, SearchOption.AllDirectories);
        return pls.Select(x => x.FullName).ToList();
    }

    /// <summary>
    /// Pre-operation For Loading Plugin
    /// </summary>
    /// <param name="pluginJsonFilePath">The Plugin Json Path</param>
    /// <exception cref="PluginImportException">Not Found Dll Or folder Or `plugin.json`</exception>
    protected virtual async Task PreOnePluginAsync(string pluginJsonFilePath)
    {
        if (!File.Exists(pluginJsonFilePath)) throw new PluginImportException($"Not Found {pluginJsonFilePath}");
        // Load Json From plugin.json
        var meta = JsonSerializer.Deserialize<TMeta>(await File.ReadAllTextAsync(pluginJsonFilePath));
        var dirPath = Path.GetDirectoryName(pluginJsonFilePath);
        if (dirPath is null || !Directory.Exists(dirPath))
        {
            // The Folder Containing The Plugin Dll Not Found
            throw new PluginImportException($"Dir Not Found: {dirPath}");
        }

        var pluginFilePath = Path.Combine(dirPath, meta!.DllName + ".dll");
        if (!File.Exists(pluginFilePath)) throw new PluginImportException($"Not Found {pluginFilePath}");
        await CheckPluginMetaDataAsync(meta!, pluginFilePath);
    }


    /// <summary>
    /// Load Plugin From Type
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    protected virtual void LoadPlugin(Type? plugin)
    {
        CheckPluginType(plugin);
        var meta = GetAndCheckPluginMetaData(plugin!);
        var instance = RegisterPluginMain(plugin!, meta);
        LoadPluginDi(plugin!, instance, meta);
        _plugins[meta.Id] = instance;
        var enabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
        instance.Loaded();
        PluginEventService.InvokePluginLoaded(this, new PluginEventArgs(meta.Id, PluginStatus.Loaded));
        Logger.Information("{Pre}{ID}: Load Success!",
            LoggerPrefix, meta.Id);
        if (!enabled) return;
        instance.IsEnabled = enabled;
    }


    /// <summary>
    /// Check Plugin Type Not Null
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <exception cref="PluginImportException">Plugin Type Is Null</exception>
    protected virtual void CheckPluginType(Type? plugin)
    {
        if (plugin is null) throw new PluginImportException("Plugin Type Not Found");
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
    protected virtual void LoadPluginType(IEnumerable<SortPluginData> sortPlugins)
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
    /// <exception cref="PluginImportException">PluginMetaData Type Is Null</exception>
    protected virtual TMeta GetAndCheckPluginMetaData(Type plugin)
    {
        var meta = plugin.GetPluginMetaData<TMeta>()
                   ?? throw new PluginImportException($"{plugin.FullName}: MetaData Not Found");
        CheckPluginMetaData(meta);
        return meta;
    }

    /// <summary>
    /// Check PluginMetaData (Async From Path)
    /// </summary>
    /// <param name="meta">PluginMetaData</param>
    /// <param name="path">Plugin Dll Path</param>
    /// <exception cref="PluginImportException">PluginMetaData Is Null</exception>
    protected virtual async Task CheckPluginMetaDataAsync(TMeta meta, string path)
    {
        if (meta is null) throw new PluginImportException($"MetaData Not Found: {path}");
        CheckPluginMetaData(meta);
        var sortData = new SortPluginData(meta.Id, meta.Dependencies);
        if (_tempSortPlugins.ContainsKey(meta.Id) || _plugins.ContainsKey(meta.Id))
        {
            // If Loaded, Next One
            Logger?.Warning("{Pre}{ID}: Exists, Continue",
                LoggerPrefix, meta.Id);
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
    /// <exception cref="PluginImportException">Can't Register Plugin</exception>
    protected virtual TAPlugin RegisterPluginMain(Type plugin, TMeta meta)
    {
        DiFactory.Services.Register(typeof(TAPlugin), plugin, Reuse.Singleton);
        var instance = DiFactory.Services.ResolveMany<TAPlugin>()
            .FirstOrDefault(x => meta.Id == x.Id);
        if (instance is null) throw new PluginImportException($"{plugin.Name}: Can't Load Plugin");
        Logger?.Information("Plugin[{ID}] Main Class Load Success", meta.Id);
        return instance;
    }

    /// <summary>
    /// Register Plugin DI
    /// </summary>
    protected virtual void LoadPluginDi(Type tPlugin, TAPlugin aPlugin, TMeta meta)
    {
    }

    /// <summary>
    /// Check plugin.json In Zip
    /// </summary>
    /// <param name="zipPath">plugin zip path</param>
    /// <exception cref="PluginImportException">Not Found plugin.json in zip</exception>
    protected virtual async Task<TMeta?> CheckPluginInZip(string zipPath)
    {
        await using FileStream zipToOpen = new(zipPath, FileMode.Open);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);
        var jsonEntry = archive.GetEntry(PluginJson) ??
                        throw new PluginImportException($"Not Found {PluginJson} in zip {zipPath}");
        using var reader = new StreamReader(jsonEntry.Open());
        var jsonContent = await reader.ReadToEndAsync();
        Logger.Information("{Pre} plugin.json content: {Content}",
            LoggerPrefix, jsonContent);
        return JsonSerializer.Deserialize<TMeta>(jsonContent);
    }

    /// <summary>
    /// UnZip
    /// </summary>
    protected virtual async Task<string> UnZip(string zipPath, string outputPath)
    {
        await using var fStream = File.OpenRead(zipPath);
        await using var stream = NonDisposingStream.Create(fStream);
        using var archive = ArchiveFactory.Open(stream);
        archive.ExtractToDirectory(outputPath);
        return outputPath;
    }
}