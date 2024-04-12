using CustomExtensions.WinUI;
using DryIoc;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace ShadowPluginLoader.WinUI;

public abstract partial class PluginLoader<M,IM, IPT> : IPluginLoader<IPT>
    where M : IM
    where IPT : IPlugin
    where IM : Attribute, IPluginMetaData
{
    /// <summary>
    /// Logger Print With Prefix
    /// </summary>
    protected string LoggerPrefix = "[PluginLoader] ";
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
    /// Log
    /// </summary>
    private ILogger? Logger { get; }
    /// <summary>
    /// Default
    /// </summary>
    /// <param name="logger">log</param>
    /// <param name="services">di services</param>
    public PluginLoader(ILogger logger, Container services)
    {
        Logger = logger;
        Services = services;
    }
    /// <summary>
    /// All Plugins
    /// </summary>
    protected readonly Dictionary<string, IPT> plugins = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// Temp Sort
    /// </summary>
    protected readonly Dictionary<string, SortPluginData> tempSortPlugins = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// Sort Loader
    /// </summary>
    protected readonly List<SortPluginData> sortLoader = new();

    

    /// <summary>
    /// 从插件文件夹中获取所有插件路径
    /// </summary>
    protected async Task GetPathFromPluginsPathAsync(DirectoryInfo dir)
    {
        var pls = dir.GetFiles(PluginPrefix + ".*.dll");
        if (pls != null && pls.Length > 0)
        {
            await LoadOnePluginAsync(pls[0].FullName);
        }
        foreach (var item in dir.GetDirectories())
        {
            await GetPathFromPluginsPathAsync(item);
        }
    }

    protected async Task LoadOnePluginAsync(string filePath)
    {
        var dirPath = Path.GetDirectoryName(filePath);
        if (dirPath is null)
        {
            Logger?.Warning("{Pre}{Path}:Can't Found",LoggerPrefix, filePath);
            return;
        }
        var json = Path.Combine(dirPath, "plugin.json");
        if (!File.Exists(json)) throw new PluginLoadError(LoggerPrefix + "Not Found plugin.json in "+ dirPath);
        var meta = JsonSerializer.Deserialize<M>(await File.ReadAllTextAsync(json));
        try
        {
            await CheckPluginMetaDataAndSortAsync(meta!, filePath);
        } 
        catch (Exception ex)
        {
            Logger?.Warning(ex.Message);
        }
    }
    

    /// <summary>
    /// 加载插件本体类
    /// </summary>
    protected void LoadPlugin(Type? plugin)
    {
        try
        {
            CheckPluginType(plugin);
            var meta = GetAndCheckPluginMetaData(plugin!);
            var instance = RegisterPluginMain(plugin!, meta);
            LoadPluginDI(plugin!, meta);
            plugins[meta.Id] = instance;
            var isEnabled = PluginStatusHelper.GetStatus(meta.Id);
            if (isEnabled) instance.Enable();
            else instance.Disable();
            Logger?.Information("{Pre}{ID}({Name}): Load Success!",
                LoggerPrefix, meta.Id, meta.Name);
        }
        catch (Exception ex)
        {
            Logger?.Warning(ex.Message);
        }
        
    }

    /// <summary>
    /// 加载插件依赖注入
    /// </summary>
    protected void LoadPluginDI(Type plugin, IM meta) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IPT? GetPlugin(string id)
    {
        if (plugins.ContainsKey(id)) return plugins[id];
        return default;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IList<IPT> GetEnabledPlugins()
    {
        var res = new List<IPT>();
        foreach (var plugin in plugins)
        {
            if (plugin.Value.IsEnabled)
            {
                res.Add(plugin.Value);
            }
        }
        return res;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IPT? GetEnabledPlugin(string id)
    {
        if (GetPlugin(id) is IPT plugin && plugin.IsEnabled) return plugin;
        return default;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IList<IPT> GetPlugins()
    {
        return plugins.Values.ToList();
    }
    protected void CheckPluginType(Type? plugin)
    {
        if (plugin is null) throw new PluginNotFoundException("Plugin Type Not Found");
    }
    protected void CheckPluginMetaData(IM meta)
    {

    }

    protected void LoadPlugins(IList<SortPluginData> sortLoader)
    {
        foreach (var data in sortLoader)
        {
            LoadPlugin(data.PluginType);
        }
    }

    protected IM GetAndCheckPluginMetaData(Type plugin)
    {
        var meta = plugin.GetPluginMetaData<IM>();
        if (meta is null) throw new PluginMetaNotFoundException($"{plugin.Name}: MetaData Not Found");
        CheckPluginMetaData(meta);
        return meta;
    }
    protected async Task CheckPluginMetaDataAndSortAsync(IM meta, string path)
    {
        if (meta is null) throw new PluginMetaNotFoundException($"MetaData Not Found");
        CheckPluginMetaData(meta);
        var sortData = new SortPluginData(meta.Id, meta.Requires);
        if (tempSortPlugins.ContainsKey(meta.Id) || plugins.ContainsKey(meta.Id))
        {
            Logger?.Warning("{Pre}{ID}({Name}): exists, continue",
            LoggerPrefix, meta.Id, meta.Name);
            return;
        }
        var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(path);
        var t = asm.ForeignAssembly.GetExportedTypes().FirstOrDefault(x => x.IsAssignableTo(typeof(IPT)));
        CheckPluginType(t);
        sortData.PluginType = t;
        sortLoader.Add(sortData);
        tempSortPlugins[sortData.Id] = sortData;
    }

    protected IPT RegisterPluginMain(Type plugin,IM meta)
    {
        Services.Register(typeof(IPT), plugin, Reuse.Singleton);
        var instance = Services.ResolveMany<IPT>()
            .FirstOrDefault(x => meta.Id == x.GetId());
        if (instance is null) throw new PluginLoadError($"{plugin.Name}: Can't Load Plugin");
        Logger?.Information("插件[{ID}]加载主类成功", meta.Id);
        return instance;
    }
}