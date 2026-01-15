using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Config;

namespace ShadowPluginLoader.WinUI.Materials;

/// <summary>
/// 网络文件原料
/// </summary>
public class HttpFileMaterial : LocalFileMaterial
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public HttpFileMaterial(Uri path) : base(path)
    {
        Raw = path;
        var baseSdkConfig = DiFactory.Services.Resolve<BaseSdkConfig>();
        var tempFile = System.IO.Path.Combine(baseSdkConfig.TempFolderPath, System.IO.Path.GetFileName(path.LocalPath));
        Path = new Uri(tempFile);
    }

    /// <inheritdoc cref="IMaterial.TypeName" />
    public override string TypeName => nameof(HttpFileMaterial);

    /// <summary>
    /// 
    /// </summary>
    public string HttpUrl => Raw.AbsoluteUri;
}