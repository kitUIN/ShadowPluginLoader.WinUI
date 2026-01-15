using System;

namespace ShadowPluginLoader.WinUI.Materials;

/// <summary>
/// 原料基类
/// </summary>
public interface IMaterial
{
    /// <summary>
    /// 原料类型名称
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// 路径
    /// </summary>
    Uri Path { get; }

    /// <summary>
    /// 原始
    /// </summary>
    Uri Raw { get; init; }
}