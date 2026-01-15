using System;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Workpieces;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 
/// </summary>
public class HttpPreprocessingProcessor : CompressedFilePreprocessingProcessor
{
    /// <inheritdoc cref="IPreprocessingProcessor.PreprocessAsync"/>
    public override async Task<IWorkpiece> PreprocessAsync(IMaterial material,
        CancellationToken cancellationToken = default)
    {
        if (material is not HttpFileMaterial httpFileMaterial)
            throw new ArgumentException("Material must be HttpFileMaterial");
        await BaseHttpHelper.Instance.SaveFileAsync(httpFileMaterial.HttpUrl, material.Path.LocalPath,
            cancellationToken: cancellationToken);
        return await base.PreprocessAsync(material, cancellationToken);
    }
}