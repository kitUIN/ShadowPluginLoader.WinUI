using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// PluginLoader
/// </summary>
/// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="AbstractPluginMetaData"/></typeparam>
/// <typeparam name="TAPlugin">Your Custom Interface IPlugin Assignable To <see cref="AbstractPlugin{TMeta}"/></typeparam>
public partial interface IPluginLoader<TMeta, TAPlugin>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    /// <param name="type">Plugin Type</param>
    IPluginLoader<TMeta, TAPlugin> Scan(Type type);

    /// <summary>
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    IPluginLoader<TMeta, TAPlugin> Scan<TPlugin>();

    /// <summary>
    /// Scan Plugins From Type<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    /// <param name="types">Plugin Type List</param>
    IPluginLoader<TMeta, TAPlugin> Scan(IEnumerable<Type> types);

    /// <summary>
    /// Scan Plugin From Optional Package<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    /// <param name="package">Optional Package</param>
    IPluginLoader<TMeta, TAPlugin> Scan(Package package);

    /// <summary>
    /// Scan Plugin From Plugin Path<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    /// <param name="pluginPath">Plugin Path</param>
    IPluginLoader<TMeta, TAPlugin> Scan(DirectoryInfo pluginPath);

    /// <summary>
    /// Scan Plugin From plugin.json<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// <example>
    /// <code>
    /// loader.Scan(pluginJson);
    /// await loader.Load();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="pluginJson">plugin.json</param>
    IPluginLoader<TMeta, TAPlugin> Scan(FileInfo pluginJson);

    /// <summary>
    /// Scan Plugin From Plugin Zip Path<br/>
    /// After Scan You Need Calling <see cref="Load"/>
    /// </summary>
    /// <param name="zipPath">Plugin Zip Path</param>
    Task ScanAsync(string zipPath);

    /// <summary>
    /// Start Load Plugin
    /// <example>
    /// <code>
    /// loader.Scan(pluginJson);
    /// await loader.Load();
    /// </code>
    /// </example>
    /// </summary>
    /// <returns></returns>
    Task Load();

    /// <summary>
    /// Get Enabled Plugins
    /// </summary>
    /// <returns>Enabled Plugins</returns>
    IList<TAPlugin> GetEnabledPlugins();

    /// <summary>
    /// Get All Plugins
    /// </summary>
    /// <returns>All Plugins</returns>
    IList<TAPlugin> GetPlugins();

    /// <summary>
    /// Get Plugin By Id
    /// </summary>
    /// <returns>Plugin or Null</returns>
    TAPlugin? GetPlugin(string id);

    /// <summary>
    /// Get Enabled Plugin By Id
    /// </summary>
    /// <returns>Plugin or Null</returns>
    TAPlugin? GetEnabledPlugin(string id);

    /// <summary>
    /// Whether The Plugin Is Enabled Or Not
    /// </summary>
    /// <param name="id">Plugin Id</param>
    /// <returns>true Or false Or null</returns>
    bool? IsEnabled(string id);

    /// <summary>
    /// Enable Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    void EnablePlugin(string id);

    /// <summary>
    /// Disable Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    void DisablePlugin(string id);

    /// <summary>
    /// Remove Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    void RemovePlugin(string id);

    /// <summary>
    /// Upgrade Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    /// <param name="newVersionZip">new Version Zip Path (Uri)</param>
    Task UpgradePlugin(string id, string newVersionZip);

    /// <summary>
    /// Checked for updates and removed plugins
    /// </summary>
    /// <returns></returns>
    Task CheckUpgradeAndRemoveAsync();
}