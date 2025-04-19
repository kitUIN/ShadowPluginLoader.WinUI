using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CustomExtensions.WinUI;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;

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
    public Dictionary<string, JsonNode?> EntryPoints { get; } = new();

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public Dictionary<string, string> DllFiles { get; } = new();


    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="PluginImportException"></exception>
    public async Task<TMeta> LoadMetaData(FileInfo pluginJson)
    {
        if (!pluginJson.Exists) throw new PluginImportException($"Not Found {pluginJson.FullName}");
        // Load Json From plugin.json

        var content = await File.ReadAllTextAsync(pluginJson.FullName);
        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new PluginDependencyJsonConverter());
        var meta = JsonSerializer.Deserialize<TMeta>(content, serializeOptions);
        EntryPoints[meta!.Id] = meta.EntryPoints;
        var dirPath = Path.GetFullPath(pluginJson.Directory!.FullName + "/../../");
        if (!Directory.Exists(dirPath))
        {
            // The Folder Containing The Plugin Dll Not Found
            throw new PluginImportException($"Dir Not Found: {dirPath}");
        }

        var dllFilePath = Path.Combine(dirPath, meta.DllName + ".dll");
        if (!File.Exists(dllFilePath)) throw new PluginImportException($"Not Found {dllFilePath}");
        DllFiles[meta.Id] = dllFilePath;
        return meta;
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
    public async Task<Type> GetMainPluginType(TMeta meta)
    {
        if (!DllFiles.TryGetValue(meta.Id, out var path))
        {
            throw new PluginImportException($"{meta.Id} Dll Path Not Found");
        }

        var assemblyName = Path.GetFileNameWithoutExtension(path);
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
        if (assembly == null)
        {
            var asm = await ApplicationExtensionHost.Current.LoadExtensionAsync(path);
            assembly = asm.ForeignAssembly;
        }

        if (!EntryPoints.TryGetValue(meta.Id, out var entryPoints) || entryPoints == null)
        {
            throw new PluginImportException($"{meta.Id} EntryPoints Not Found");
        }

        if (!entryPoints.AsObject()
                .TryGetPropertyValue("MainPlugin", out var mainTypeName) || mainTypeName == null)
        {
            throw new PluginImportException($"{meta.Id} MainPlugin(EntryPoint) Not Found");
        }

        var mainType = assembly.GetType(mainTypeName.GetValue<string>());
        if (mainType == null)
        {
            throw new PluginImportException($"{meta.Id} MainPlugin(Type) Not Found");
        }

        return mainType;
    }
}