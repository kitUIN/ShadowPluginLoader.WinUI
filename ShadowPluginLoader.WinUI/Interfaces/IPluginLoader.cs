using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    /// <param name="type">Plugin Type</param>
    IPluginLoader<TMeta, TAPlugin> Scan(Type? type);

    /// <summary>
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    IPluginLoader<TMeta, TAPlugin> Scan<TPlugin>();

    /// <summary>
    /// Scan Plugins From Type<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    /// <param name="types">Plugin Type List</param>
    IPluginLoader<TMeta, TAPlugin> Scan(IEnumerable<Type> types);

    /// <summary>
    /// Scan Plugin From Optional Package<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    /// <param name="package">Optional Package</param>
    IPluginLoader<TMeta, TAPlugin> Scan(Package package);

    /// <summary>
    /// Scan Plugin From Plugin Path<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    /// <param name="pluginPath">Plugin Path</param>
    IPluginLoader<TMeta, TAPlugin> Scan(DirectoryInfo pluginPath);

    /// <summary>
    /// Scan Plugin From plugin.json<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// <example>
    /// <code>
    /// loader.Scan(pluginJson);
    /// await loader.LoadAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="pluginJson">plugin.json</param>
    IPluginLoader<TMeta, TAPlugin> Scan(FileInfo pluginJson);

    /// <summary>
    /// Scan Plugin From Uri<br/>
    /// After Scan You Need Calling <see cref="LoadAsync"/>
    /// </summary>
    /// <param name="uri">Uri</param>
    IPluginLoader<TMeta, TAPlugin> Scan(Uri uri);

    /// <summary>
    /// Clear Scan History
    /// </summary>
    void ScanClear();

    /// <summary>
    /// Start LoadAsync Plugin
    /// <example>
    /// <code>
    /// loader.Scan(pluginJson);
    /// await loader.LoadAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>Need Upgrade Plugin</returns>
    Task<List<SortPluginData<TMeta>>> LoadAsync();

    /// <summary>
    /// 预加载插件DLL（不实例化）
    /// <example>
    /// <code>
    /// loader.Scan(pluginJson);
    /// await loader.PreloadAsync(); // 只加载DLL，不实例化
    /// await loader.InstantiatePluginsAsync(); // 延迟实例化
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>预加载的插件数据</returns>
    Task<List<PreloadedPluginData<TMeta>>> PreloadAsync();

    /// <summary>
    /// 实例化预加载的插件
    /// </summary>
    /// <param name="pluginIds">要实例化的插件ID列表，为空则实例化所有</param>
    /// <returns>实例化的插件数量</returns>
    Task<int> InstantiatePluginsAsync(IEnumerable<string>? pluginIds = null);

    /// <summary>
    /// 获取预加载的插件数据
    /// </summary>
    /// <returns>预加载的插件数据列表</returns>
    IList<PreloadedPluginData<TMeta>> GetPreloadedPlugins();

    /// <summary>
    /// 获取指定插件的预加载数据
    /// </summary>
    /// <param name="id">插件ID</param>
    /// <returns>预加载的插件数据或null</returns>
    PreloadedPluginData<TMeta>? GetPreloadedPlugin(string id);

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
    Task RemovePlugin(string id);

    /// <summary>
    /// Upgrade Plugin
    /// </summary>
    /// <param name="id">Plugin Id</param>
    /// <param name="uri">new Version Path (Uri)</param>
    Task UpgradePlugin(string id, Uri uri);

    /// <summary>
    /// Checked for updates and removed plugins
    /// </summary>
    /// <returns></returns>
    Task CheckUpgradeAndRemoveAsync();
}