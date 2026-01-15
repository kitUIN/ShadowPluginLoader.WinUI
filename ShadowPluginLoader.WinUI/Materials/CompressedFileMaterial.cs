using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Config;

namespace ShadowPluginLoader.WinUI.Materials;

/// <summary>
/// 压缩包材料
/// </summary>
public class CompressedFileMaterial : LocalFileMaterial
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public CompressedFileMaterial(Uri path) : base(path)
    {
    }

    /// <inheritdoc cref="IMaterial.TypeName" />
    public override string TypeName => nameof(CompressedFileMaterial);
    
}