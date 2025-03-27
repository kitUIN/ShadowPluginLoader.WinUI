using CustomExtensions.WinUI;
using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Interfaces;
using SharpCompress.Archives;
using SharpCompress.IO;
using SharpCompress.Readers;
using Path = System.IO.Path;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly Queue<FileInfo> ScanQueue = new();

    /// <summary>
    /// Clean Folder Before Upgrade
    /// </summary>
    protected virtual bool CleanBeforeUpgrade => false;

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
    /// Temp File Folder
    /// </summary>
    protected abstract string TempFolder { get; }

    /// <summary>
    /// DependencyChecker
    /// </summary>
    protected virtual IDependencyChecker<TMeta> DependencyChecker { get; } = new DependencyChecker<TMeta>();

    /// <summary>
    /// MetaDataChecker
    /// </summary>
    protected virtual IMetaDataChecker<TMeta> MetaDataChecker { get; } = new MetaDataChecker<TMeta>();

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
    /// Load Plugin From Type
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <param name="meta">Plugin MetaData</param>
    protected virtual void LoadPlugin(Type plugin, TMeta meta)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            // CheckPluginType(plugin);
            MetaDataChecker.CheckMetaDataValid(meta);
            BeforeLoadPlugin(plugin, meta);
            var instance = LoadMainPlugin(plugin, meta);
            AfterLoadPlugin(plugin, instance, meta);
            _plugins[meta.Id] = instance;
            var enabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
            instance.Loaded();
            PluginEventService.InvokePluginLoaded(this, new PluginEventArgs(meta.Id, PluginStatus.Loaded));
            stopwatch.Stop();
            Logger.Information("{Pre}{ID}({isEnabled}): Load Success! Used: {mi} ms",
                LoggerPrefix, meta.Id, enabled, stopwatch.ElapsedMilliseconds);
            if (!enabled) return;
            instance.IsEnabled = enabled;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
                Logger.Information("{Pre}Plugin Load Failed! Used: {mi} ms",
                    LoggerPrefix, stopwatch.ElapsedMilliseconds);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="plugin"></param>
    /// <param name="meta"></param>
    protected virtual void BeforeLoadPlugin(Type plugin, TMeta meta)
    {
    }


    /// <summary>
    /// Register Plugin Main
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <param name="meta">PluginMetaData</param>
    /// <returns>Plugin Instance</returns>
    /// <exception cref="PluginImportException">Can't Register Plugin</exception>
    protected virtual TAPlugin LoadMainPlugin(Type plugin, TMeta meta)
    {
        DiFactory.Services.Register(typeof(TAPlugin), plugin, reuse: Reuse.Singleton,
            serviceKey: meta.Id);
        var instance = DiFactory.Services.Resolve<TAPlugin>(serviceKey: meta.Id);
        if (instance is null) throw new PluginImportException($"{plugin.Name}: Can't Load Plugin");
        Logger?.Information("Plugin[{ID}] Main Class Load Success", meta.Id);
        return instance;
    }

    /// <summary>
    /// After Load Plugin
    /// </summary>
    protected virtual void AfterLoadPlugin(Type tPlugin, TAPlugin aPlugin, TMeta meta)
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
        var jsonEntry = archive.Entries.FirstOrDefault(entry =>
            entry.FullName.EndsWith("/Assets/plugin.json", StringComparison.OrdinalIgnoreCase));
        if (jsonEntry == null) throw new PluginImportException($"Not Found plugin.json in zip {zipPath}");
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