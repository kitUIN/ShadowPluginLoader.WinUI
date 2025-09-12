namespace ShadowPluginLoader.WinUI.Enums;

/// <summary>
/// 插件加载阶段
/// </summary>
public enum PluginLoadPhase
{
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted,

    /// <summary>
    /// DLL预加载阶段 - 只加载程序集，不实例化
    /// </summary>
    Preloaded,

    /// <summary>
    /// 实例化阶段 - 创建插件实例
    /// </summary>
    Instantiated,

    /// <summary>
    /// 已启用
    /// </summary>
    Enabled,

    /// <summary>
    /// 已禁用
    /// </summary>
    Disabled,

    /// <summary>
    /// 加载失败
    /// </summary>
    Failed
}