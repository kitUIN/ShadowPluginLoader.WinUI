using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Helpers;
using SharpCompress.Archives;
using SharpCompress.IO;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// PluginInstaller For Zip/Rar/Tat and more
/// </summary>
public partial class ZipPluginInstaller : IPluginInstaller
{
    /// <summary>
    /// Logger
    /// </summary>
    [Autowired]
    public ILogger Logger { get; }

    /// <inheritdoc />
    public int Priority => 1;

    /// <summary>
    /// SupportTypes
    /// </summary>
    protected string[] SupportTypes => [".zip", ".rar"];

    /// <inheritdoc />
    public bool Check(Uri path)
    {
        return SupportTypes.Any(x => path.OriginalString.EndsWith(x, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public async Task<FileInfo> ScanAsync(Uri uri, string tempFolder, string targetFolder)
    {
        var zipPath = await DownloadHelper.DownloadFileAsync(tempFolder, uri.AbsoluteUri, Logger);
        var outName = Path.GetFileNameWithoutExtension(zipPath);
        var outPath = Path.Combine(targetFolder, outName);
        var count = 0;
        while (Directory.Exists(outPath))
        {
            outPath = Path.Combine(targetFolder, $"{outName}{++count}");
        }
        Logger.Debug("Plugin OutPath: {t}", outPath);
        var dir = await UnZip(zipPath, outPath);
        var jsonEntryFile = new DirectoryInfo(dir)
            .GetDirectories("Assets", SearchOption.AllDirectories)
            .Select(assetDir => new FileInfo(Path.Combine(assetDir.FullName, "plugin.json")))
            .FirstOrDefault(file => file.Exists);
        if (jsonEntryFile == null) throw new PluginImportException($"Not Found plugin.json in zip {zipPath}");
        return jsonEntryFile;
    }

    /// <summary>
    /// UnZip
    /// </summary>
    protected virtual async Task<string> UnZip(string zipPath, string outputPath)
    {
        await using var fStream = File.OpenRead(zipPath);
        await using var stream = NonDisposingStream.Create(fStream);
        using var archive = ArchiveFactory.Open(stream);
        archive.ExtractToDirectory(outputPath);
        return outputPath;
    }

    /// <inheritdoc />
    public async Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder)
    {
        await UnZip(uri.AbsoluteUri, targetFolder);
    }

    /// <inheritdoc />
    public bool Remove(string pluginId)
    {
        throw new System.NotImplementedException();
    }
}