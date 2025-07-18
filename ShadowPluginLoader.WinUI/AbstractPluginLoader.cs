using DryIoc;
using Serilog;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Services;
using SharpCompress.Archives;
using SharpCompress.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// checked for updates and removed plugins
    /// </summary>
    protected bool IsCheckUpgradeAndRemove = false;

    /// <summary>
    /// Clean Folder Before Upgrade
    /// </summary>
    protected virtual bool CleanBeforeUpgrade => false;

    /// <summary>
    /// Logger Print With Prefix
    /// </summary>
    protected virtual string LoggerPrefix => "[PluginLoader] ";


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
    /// LoadAsync Plugin From Type
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <param name="meta">Plugin MetaData</param>
    protected virtual void LoadPlugin(Type plugin, TMeta meta)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            BeforeLoadPlugin(plugin, meta);
            var instance = LoadMainPlugin(plugin, meta);
            AfterLoadPlugin(plugin, instance, meta);
            _plugins[meta.Id] = instance;
            var enabled = PluginSettingsHelper.GetPluginIsEnabled(meta.Id);
            instance.Loaded();
            PluginEventService.InvokePluginLoaded(this, new PluginEventArgs(meta.Id, PluginStatus.Loaded));
            stopwatch.Stop();
            Logger.Information("{Pre}{ID}({isEnabled}): LoadAsync Success! Used: {mi} ms",
                LoggerPrefix, meta.Id, enabled, stopwatch.ElapsedMilliseconds);
            DependencyChecker.LoadedPlugins.Add(meta.DllName, new Version(meta.Version));
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
        DiFactory.Services.Register(typeof(TAPlugin), plugin, reuse: Reuse.Singleton,
            serviceKey: meta.Id);
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