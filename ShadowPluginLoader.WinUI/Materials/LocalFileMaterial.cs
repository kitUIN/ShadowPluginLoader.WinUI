using System;

namespace ShadowPluginLoader.WinUI.Materials;

/// <summary>
/// 本地文件材料
/// </summary>
public class LocalFileMaterial : IMaterial
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public LocalFileMaterial(Uri path)
    {
        Raw = path;
        Path = path;
    } 

    /// <inheritdoc/>
    public virtual string TypeName => nameof(LocalFileMaterial);

    /// <inheritdoc/>
    public Uri Path { get; init; }

    /// <summary>
    /// 原始路径
    /// </summary>
    public Uri Raw { get; init; }
}