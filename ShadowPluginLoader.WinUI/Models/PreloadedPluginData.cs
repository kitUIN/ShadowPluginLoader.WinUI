using System;
using System.Collections.Generic;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// 预加载的插件数据，包含DLL已加载但未实例化的插件信息
/// </summary>
/// <typeparam name="TMeta">插件元数据类型</typeparam>
public class PreloadedPluginData<TMeta> where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// 插件元数据
    /// </summary>
    public TMeta MetaData { get; init; } = null!;

    /// <summary>
    /// 插件主类型
    /// </summary>
    public Type MainPluginType { get; init; } = null!;

    /// <summary>
    /// 插件程序集
    /// </summary>
    public System.Reflection.Assembly Assembly { get; init; } = null!;

    /// <summary>
    /// 插件安装器ID
    /// </summary>
    public string InstallerId { get; init; } = null!;

    /// <summary>
    /// 插件路径
    /// </summary>
    public string Path { get; init; } = null!;

    /// <summary>
    /// 是否已实例化
    /// </summary>
    public bool IsInstantiated { get; set; } = false;

    /// <summary>
    /// 实例化时间
    /// </summary>
    public DateTime? InstantiatedAt { get; set; }

    /// <summary>
    /// 插件实例（实例化后）
    /// </summary>
    public AbstractPlugin<TMeta>? Instance { get; set; }

    /// <summary>
    /// 依赖关系
    /// </summary>
    public List<PluginDependency> Dependencies { get; init; } = new();

    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// 版本
    /// </summary>
    public Version Version { get; init; } = null!;
}