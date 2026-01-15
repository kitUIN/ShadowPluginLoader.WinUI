using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Workpieces;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 
/// </summary>
public class CompressedFilePreprocessingProcessor : IPreprocessingProcessor
{
    /// <inheritdoc cref="IPreprocessingProcessor.PreprocessAsync"/>
    public virtual async Task<IWorkpiece> PreprocessAsync(IMaterial material,
        CancellationToken cancellationToken = default)
    {
        await using FileStream zipToOpen = new(material.Path.LocalPath, FileMode.Open);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
        var entry = archive.Entries.FirstOrDefault(e =>
            e.FullName.EndsWith("/plugin.json", StringComparison.OrdinalIgnoreCase));
        if (entry == null) throw new PluginLoadMetaException($"Not Found plugin.json in zip {material.Path.LocalPath}");
        using var reader = new StreamReader(entry.Open());
        var jsonContent = await reader.ReadToEndAsync();
        return new CompressedWorkpiece(jsonContent, material.Path);
    }
}