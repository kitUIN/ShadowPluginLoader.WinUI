using System;

namespace ShadowPluginLoader.WinUI.Workpieces;

/// <summary>
/// 空白工件
/// </summary>
public class LocalWorkpiece : IWorkpiece
{
    /// <summary>
    /// 构造空白工件
    /// </summary>
    /// <param name="metaData">元数据</param>
    /// <param name="path">路径</param>
    public LocalWorkpiece(string metaData, Uri path) => (MetaData, Path) = (metaData, path);

    /// <inheritdoc/>
    public string MetaData { get; init; }

    /// <inheritdoc/>
    public Uri Path { get; init; }
}