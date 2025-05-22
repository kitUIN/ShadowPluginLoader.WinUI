using CustomExtensions.WinUI;
using Serilog.Core;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Serilog;
using ShadowPluginLoader.WinUI.Extensions;

namespace ShadowPluginLoader.WinUI.Checkers;

/// <summary>
/// 
/// </summary>
public class MetaDataChecker<TMeta> : IMetaDataChecker<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public ConcurrentDictionary<string, PluginEntryPoint[]> EntryPoints { get; } = new();


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public async Task<SortPluginData<TMeta>> LoadSortPluginData(Uri uri, string tempFolder)
    {
        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new PluginDependencyJsonConverter());
        var zipPath = await FileHelper.DownloadFileAsync(tempFolder, uri,
            Log.ForContext<MetaDataChecker<AbstractPluginMetaData>>());
        if (zipPath.IsZip())
        {
            await using FileStream zipToOpen = new(zipPath.LocalPath, FileMode.Open);
            using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
            var entry = archive.Entries.FirstOrDefault(e =>
                e.FullName.EndsWith("/plugin.json", StringComparison.OrdinalIgnoreCase));

            if (entry == null) throw new PluginImportException($"Not Found plugin.json in zip {zipPath}");
            using var reader = new StreamReader(entry.Open());
            var jsonContent = await reader.ReadToEndAsync();

            var zipMeta = JsonSerializer.Deserialize<TMeta>(jsonContent, serializeOptions);
            EntryPoints[zipMeta!.Id] = zipMeta.EntryPoints;
            return new SortPluginData<TMeta>(zipMeta, zipPath);
        }

        if (!uri.IsFile || !uri.AbsolutePath.EndsWith("plugin.json"))
            throw new PluginImportException($"Not Found  plugin.json in {uri}");
        var pluginJson = new FileInfo(uri.LocalPath);
        if (!pluginJson.Exists) throw new PluginImportException($"Not Found {pluginJson.FullName}");
        // LoadAsync Json From plugin.json

        var content = await File.ReadAllTextAsync(pluginJson.FullName);

        var meta = JsonSerializer.Deserialize<TMeta>(content, serializeOptions);
        EntryPoints[meta!.Id] = meta.EntryPoints;
        return new SortPluginData<TMeta>(meta, uri);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zip"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public TMeta LoadMetaData(string zip)
    {
        // TODO
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meta"></param>
    public void CheckMetaDataValid(TMeta meta)
    {
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public async Task<Type> GetMainPluginType(SortPluginData<TMeta> sortPluginData)
    {
        var dirPath = Path.GetFullPath(new FileInfo(sortPluginData.Path).Directory!.FullName + "/../../");
        if (!Directory.Exists(dirPath))
        {
            // The Folder Containing The Plugin Dll Not Found
            throw new PluginImportException($"Dir Not Found: {dirPath}");
        }

        var dllFilePath = Path.Combine(dirPath, sortPluginData.MetaData.DllName + ".dll");
        if (!File.Exists(dllFilePath)) throw new PluginImportException($"Not Found {dllFilePath}");


        var assemblyName = Path.GetFileNameWithoutExtension(dllFilePath);
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
        if (assembly == null)
        {
            var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(dllFilePath);
            assembly = asm.ForeignAssembly;
        }

        var entryPoints = sortPluginData.MetaData.EntryPoints;
        if (entryPoints.FirstOrDefault(x => x.Name == "MainPlugin") is not { } pluginEntryPoint)
        {
            throw new PluginImportException($"{sortPluginData.MetaData.Id} MainPlugin(EntryPoint) Not Found");
        }

        var mainType = assembly.GetType(pluginEntryPoint.Type);
        if (mainType == null)
        {
            throw new PluginImportException($"{sortPluginData.MetaData.Id} MainPlugin(Type) Not Found");
        }

        return mainType;
    }
}