using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Scanners;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{

    /// <summary>
    /// Logger Print With Prefix
    /// </summary>
    protected virtual string LoggerPrefix => "[PluginLoader] ";

    /// <summary>
    /// Base Folder
    /// </summary>
    public string BaseFolder { get; } = ApplicationData.Current.LocalFolder.Path;

    /// <summary>
    /// Plugin Folder Path
    /// </summary>
    public string PluginFolderPath { get; }

    /// <summary>
    /// Plugin Folder Path
    /// </summary>
    public string TempFolderPath { get; }

    /// <summary>
    /// Plugins Folder
    /// </summary>
    protected const string PluginFolder = "plugin";

    /// <summary>
    /// Temp File Folder
    /// </summary>
    protected const string TempFolder = "temp";

    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// PluginEventService
    /// </summary>
    protected PluginEventService PluginEventService { get; }


    /// <summary>
    /// Get PluginInstaller
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    protected IPluginInstaller GetPluginInstaller(Uri uri)
    {
        foreach (var installer in DiFactory.Services.ResolveMany<IPluginInstaller>().OrderBy(x => x.Priority))
        {
            try
            {
                if (installer.Check(uri)) return installer;
            }
            catch (Exception e)
            {
                Logger.Warning("PluginInstaller CheckError: {Ex}", e);
            }
        }

        return new BasePluginInstaller(Logger);
    }

    /// <summary>
    /// Get PluginInstaller
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected IPluginInstaller GetPluginInstaller(string key)
    {
        foreach (var installer in DiFactory.Services.ResolveMany<IPluginInstaller>())
        {
            try
            {
                if (installer.Identify == key) return installer;
            }
            catch (Exception e)
            {
                Logger.Warning("PluginInstaller CheckError: {Ex}", e);
            }
        }

        return new BasePluginInstaller(Logger);
    }

    /// <summary>
    /// Default
    /// </summary>
    /// <param name="logger">log</param>
    /// <param name="pluginEventService">pluginEventService</param>
    protected AbstractPluginLoader(ILogger logger, PluginEventService pluginEventService)
    {
        PluginFolderPath = Path.Combine(BaseFolder, PluginFolder);
        TempFolderPath = Path.Combine(BaseFolder, TempFolder);
        PluginScanner = new PluginScanner<TAPlugin, TMeta>(DependencyChecker, UpgradeChecker, RemoveChecker);
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
    private readonly Dictionary<string, TAPlugin> _plugins = new();

    protected virtual void LoadConfigFile()
    {

    }

    /// <summary>
    /// LoadAsync Plugin From Type
    /// </summary>
    /// <param name="meta">Plugin MetaData</param>
    protected virtual void LoadPlugin(TMeta meta)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            BeforeLoadPlugin(meta.MainPluginType, meta);
            var instance = LoadMainPlugin(meta.MainPluginType, meta);
            AfterLoadPlugin(meta.MainPluginType, instance, meta);
            _plugins[meta.Id] = instance;
            var enabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
            instance.Loaded();
            PluginEventService.InvokePluginLoaded(this, new PluginEventArgs(meta.Id, PluginStatus.Loaded));
            stopwatch.Stop();
            Logger.Information("{Pre}{ID}({isEnabled}): LoadAsync Success! Used: {mi} ms",
                LoggerPrefix, meta.Id, enabled, stopwatch.ElapsedMilliseconds);
            DependencyChecker.LoadedPlugins.Add(meta.DllName, meta.Version);
            if (!enabled) return;
            instance.IsEnabled = enabled;
        }
        catch (Exception e)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            Logger.Warning("{Pre}Plugin LoadAsync Failed! Used: {mi} ms, Error: {Ex}",
                LoggerPrefix, stopwatch.ElapsedMilliseconds, e);
            throw;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
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
        var instance = DiFactory.Services.Resolve<TAPlugin>(serviceKey: meta.Id);
        if (instance is null) throw new PluginImportException($"{plugin.Name}: Can't LoadAsync Plugin");
        Logger.Information("Plugin[{ID}] Main Class LoadAsync Success", meta.Id);
        return instance;
    }

    /// <summary>
    /// After LoadAsync Plugin
    /// </summary>
    protected virtual void AfterLoadPlugin(Type tPlugin, TAPlugin aPlugin, TMeta meta)
    {
    }
}