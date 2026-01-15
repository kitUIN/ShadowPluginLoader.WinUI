using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Models;

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
    /// Get All Plugins
    /// </summary>
    /// <returns>All Plugins</returns>
    IList<TAPlugin> GetPlugins();

    /// <summary>
    /// Get All Plugins
    /// </summary>
    /// <returns>All Plugins</returns>
    IList<TAPlugin> GetPlugins(bool isEnabled);

    /// <summary>
    /// Get Plugin By Id
    /// </summary>
    /// <returns>Plugin or Null</returns>
    TAPlugin? GetPlugin(string id);

    /// <summary>
    /// Get Plugin By Id
    /// </summary>
    /// <returns>Plugin or Null</returns>
    TAPlugin? GetPlugin(string id, bool isEnabled);


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
    /// Upgrade Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    /// <param name="uri">new Version Path (Uri)</param>
    Task UpgradePlugin(string id, Uri uri);

    /// <summary>
    /// Load
    /// </summary>
    /// <returns></returns>
    int Load(IEnumerable<string> pluginIds, IProgress<PipelineProgress>? progress = null);
}