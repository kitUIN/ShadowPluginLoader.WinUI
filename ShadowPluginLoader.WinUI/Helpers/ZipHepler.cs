using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// 
/// </summary>
public static class ZipHelper
{
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zipPath"></param>
    /// <param name="outputDir"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    public static async Task UnZip(string zipPath, string outputDir,
        IProgress<ProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var options = new ReaderOptions
        {
            LeaveStreamOpen = false,
            ArchiveEncoding = new ArchiveEncoding
            {
                Default = Encoding.UTF8
            }
        };
        using var archive = ArchiveFactory.Open(zipPath, options);
        if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
        await archive.WriteToDirectoryAsync(outputDir, new ExtractionOptions
        {
            ExtractFullPath = true,
            Overwrite = true,
        }, progress, cancellationToken);
    }
    /// <summary>
    /// UnZip
    /// </summary>
    public static async Task<Uri> UnZip(string zipPath, string baseFolder, string dllName,
        IProgress<ProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var outputDir = Path.Combine(baseFolder, dllName);
        var outputUri = new Uri(Path.Combine(outputDir, dllName, "plugin.json"));
        await UnZip(zipPath, outputDir, progress, cancellationToken);
        return outputUri;
    }
}