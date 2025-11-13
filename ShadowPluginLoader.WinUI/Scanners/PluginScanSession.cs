using Serilog;
using Serilog.Core;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ShadowPluginLoader.WinUI.Scanners;

/// <summary>
/// 
/// </summary>
public class PluginScanSession<TAPlugin, TMeta> : IPluginScanSession<TAPlugin, TMeta>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    private readonly IPluginScanner<TAPlugin, TMeta> _scanner;
    private readonly Guid _token;

    internal PluginScanSession(IPluginScanner<TAPlugin, TMeta> scanner, Guid token)
    {
        _scanner = scanner;
        _token = token;
    }

    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; } = Log.ForContext("SourceContext", "S.W.S.PluginScanSession");


    /// <summary>
    /// Used to store Tasks that convert JSON files to TMeta
    /// </summary>
    protected readonly ConcurrentBag<Task<SortPluginData<TMeta>>> ScanTaskList = [];

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(Type? type)
    {
        if (type is null) return this;
        var dir = type.Assembly.Location[..^".dll".Length];
        var metaPath = Path.Combine(dir, "plugin.json");
        Scan(new Uri(metaPath));
        return this;
    }


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan<TPlugin>()
    {
        Scan(typeof(TPlugin));
        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(params Type?[] types)
    {
        foreach (var type in types)
        {
            Scan(type);
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(IEnumerable<Type?> types)
    {
        foreach (var type in types)
        {
            Scan(type);
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(Package package)
    {
        return Scan(new DirectoryInfo(package.InstalledPath));
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(DirectoryInfo dir)
    {
        if (!dir.Exists)
        {
            Logger?.Warning("Scan Dir[{DirFullName}]: Dir Not Exists", dir.FullName);
            return this;
        }

        foreach (var pluginFile in dir.EnumerateFiles("plugin.json", SearchOption.AllDirectories))
        {
            Scan(new Uri(pluginFile.FullName));
        }

        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(FileInfo pluginJson)
    {
        if (File.Exists(pluginJson.FullName)) Scan(new Uri(pluginJson.FullName));
        return this;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IPluginScanSession<TAPlugin, TMeta> Scan(Uri uri)
    {
        if (!uri.IsFile && Directory.Exists(uri.LocalPath))
        {
            Scan(new DirectoryInfo(uri.LocalPath));
        }
        else if (uri.IsFile && uri.LocalPath.EndsWith("plugin.json"))
        {
            Logger?.Information("Scan Uri[{DirFullName}]: Success", uri.LocalPath);
            ScanTaskList.Add(Task.Run(async () =>
            {
                var content = await File.ReadAllTextAsync(uri.LocalPath);
                var meta = MetaDataHelper.ToMeta<TMeta>(content) ??
                           throw new PluginScanException($"Failed to deserialize plugin metadata from {uri}");
                return new SortPluginData<TMeta>(meta, uri);
            }));
        }
        else
        {
            Logger?.Warning("Scan Uri[{DirFullName}]: Not Support", uri.LocalPath);
        }

        return this;
    }

    /// <inheritdoc />
    public void ScanClear()
    {
        ScanTaskList.Clear();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> FinishAsync()
    {
        var scanTaskArray = ScanTaskList.ToArray();
        ScanClear();
        if (scanTaskArray.Length == 0) return [];
        return await _scanner.FinishScanAsync(scanTaskArray, _token);
    }
}