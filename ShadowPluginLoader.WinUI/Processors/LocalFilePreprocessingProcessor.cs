using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Workpieces;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 本地文件预处理
/// </summary>
public class LocalFilePreprocessingProcessor : IPreprocessingProcessor
{
    /// <inheritdoc/>
    public virtual async Task<IWorkpiece> PreprocessAsync(IMaterial material,
        CancellationToken cancellationToken = default)
    {
        var content = await File.ReadAllTextAsync(material.Path.LocalPath, cancellationToken);
        return new LocalWorkpiece(content, material.Path);
    }
}