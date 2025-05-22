using Serilog;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Extensions;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Readers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// PluginInstaller For Zip/Rar/Tat and more
/// </summary>
public partial class ZipPluginInstaller : BasePluginInstaller
{
    /// <summary>
    /// Logger
    /// </summary>
    [Autowired]
    public ILogger Logger { get; }

    /// <inheritdoc />
    public override int Priority => 1;

    /// <inheritdoc />
    public override bool Check(Uri path)
    {
        return path.IsZip();
    }

    /// <inheritdoc />
    public override async Task<SortPluginData<TMeta>> InstallAsync<TMeta>(SortPluginData<TMeta> sortPluginData,
        string tempFolder, string pluginFolder)
    {
        var outPath = FileHelper.GetName(sortPluginData.Path, pluginFolder, true);
        Logger.Debug("Plugin OutPath: {t}", outPath);
        var dir = await UnZip(sortPluginData.Path, outPath);
        var jsonEntryFile = new DirectoryInfo(dir)
            .GetDirectories("Assets", SearchOption.AllDirectories)
            .Select(assetDir => new FileInfo(Path.Combine(assetDir.FullName, "plugin.json")))
            .FirstOrDefault(file => file.Exists);
        if (jsonEntryFile == null)
            throw new PluginImportException($"Not Found plugin.json in zip {sortPluginData.Path}");
        return new SortPluginData<TMeta>(sortPluginData.MetaData, jsonEntryFile.FullName);
    }

    /// <summary>
    /// UnZip
    /// </summary>
    protected virtual Task<string> UnZip(string zipPath, string outputPath)
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
        if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            entry.WriteToDirectory(outputPath, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true,
            });
        }

        return Task.FromResult(outputPath);
    }

    /// <inheritdoc />
    public override async Task<string?> PreUpgradeAsync(string pluginId, Uri uri, string tempFolder,
        string targetFolder)
    {
        var newVersionUri = await FileHelper.DownloadFileAsync(tempFolder, uri, Logger);
        return newVersionUri.LocalPath;
    }

    /// <inheritdoc />
    public override async Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetPath)
    {
        await UnZip(uri.LocalPath, targetPath);
    }
}