using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// PluginLoader Interface
/// </summary>
/// <typeparam name="TAPlugin">Plugin Base Interface, Default: <see cref="APlugin"/></typeparam>
public partial interface IPluginLoader<TAPlugin> 
    where TAPlugin: APlugin
{

    /// <summary>
    /// Import Plugin From Type
    /// </summary>
    /// <param name="type">Plugin Type</param>
    void Import(Type type);
    /// <summary>
    /// Import Plugins From Type
    /// </summary>
    /// <param name="types">Plugin Type List</param>
    void Import(IEnumerable<Type> types);
    
    /// <summary>
    /// Import Plugin From Plugin Path
    /// </summary>
    /// <param name="pluginPath">Plugin Path</param>
    Task ImportAsync(string pluginPath);
    /// <summary>
    /// Import Plugin From Plugin Zip Path
    /// </summary>
    /// <param name="zipPath">Plugin Zip Path</param>
    Task ImportFromZipAsync(string zipPath);
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
    /// Delete Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    void DeletePlugin(string id);
    
    /// <summary>
    /// Upgrade Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    void UpgradePlugin(string id);
}