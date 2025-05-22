using CustomExtensions.WinUI;
using Serilog;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Base Plugin Installer
/// </summary>
public partial class BasePluginInstaller : IPluginInstaller
{
    /// <summary>
    /// Logger
    /// </summary>
    [Autowired]
    public ILogger Logger { get; }

    /// <inheritdoc />
    public virtual int Priority => 100;

    /// <inheritdoc />
    public virtual string Identify => "Base";

    /// <inheritdoc />
    public virtual bool Check(Uri path)
    {
        return true;
    }

    /// <inheritdoc />
    public virtual Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData,
        string tempFolder, string pluginFolder) where TMeta : AbstractPluginMetaData
    {
        PluginSettingsHelper.SetPluginInstaller(sortPluginData.Id, Identify);
        return Task.FromResult(sortPluginData);
    }

    /// <inheritdoc />
    public virtual Task PreUpgradeAsync<TMeta>(AbstractPlugin<TMeta> plugin, Uri uri, string tempFolder,
        string targetFolder) where TMeta : AbstractPluginMetaData
    {
        return Task.FromResult(uri.LocalPath)!;
    }


    /// <inheritdoc />
    public virtual Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath)
    {
        PluginSettingsHelper.DeleteUpgradePluginPath(pluginId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task PreRemoveAsync<TMeta>(AbstractPlugin<TMeta> plugin, string tempFolder, string targetFolder)
        where TMeta : AbstractPluginMetaData
    {
        plugin.PlanRemove = true;
        PluginSettingsHelper.SetPluginPlanRemove(plugin.Id, targetFolder);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task RemoveAsync(string pluginId, string tempFolder, string targetPath)
    {
        if (Directory.Exists(targetPath)) Directory.Delete(targetPath, true);
        PluginSettingsHelper.DeleteRemovePluginPath(pluginId);
        return Task.CompletedTask;
    }


    /// <inheritdoc />
    public ConcurrentDictionary<string, PluginEntryPoint[]> EntryPoints { get; } = new();

    /// <inheritdoc />
    public virtual async Task<SortPluginData<TMeta>> LoadSortPluginData<TMeta>(Uri uri, string tempFolder)
        where TMeta : AbstractPluginMetaData
    {
        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new PluginDependencyJsonConverter());
        if (!uri.IsFile || !uri.AbsolutePath.EndsWith("plugin.json"))
            throw new PluginImportException($"Not Found  plugin.json in {uri}");
        var pluginJson = new FileInfo(uri.LocalPath);
        if (!pluginJson.Exists) throw new PluginImportException($"Not Found {pluginJson.FullName}");
        // LoadAsync Json From plugin.json

        var content = await File.ReadAllTextAsync(pluginJson.FullName);

        var meta = JsonSerializer.Deserialize<TMeta>(content, serializeOptions);
        EntryPoints[meta!.Id] = meta.EntryPoints;
        return new SortPluginData<TMeta>(meta, uri, Identify);
    }

    /// <inheritdoc />
    public virtual async Task<Type> GetMainPluginType<TMeta>(SortPluginData<TMeta> sortPluginData)
        where TMeta : AbstractPluginMetaData
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