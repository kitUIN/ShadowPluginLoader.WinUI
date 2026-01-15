using System;

namespace ShadowPluginLoader.WinUI.Workpieces;

/// <summary>
/// 工件
/// </summary>
public interface IWorkpiece
{
    /// <summary>
    /// 元数据
    /// </summary>
    string MetaData { get; init; }

    /// <summary>
    /// 路径
    /// </summary>
    Uri Path { get; init; }
}