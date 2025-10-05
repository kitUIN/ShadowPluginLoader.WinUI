using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Installer;

/// <inheritdoc />
public partial class ZipPluginInstaller<TMeta> : IPluginInstaller<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected IDependencyChecker<TMeta> DependencyChecker { get; }

    /// <summary>
    /// 
    /// </summary>
    [Autowired]
    protected BaseSdkConfig BaseSdkConfig { get; }

    /// <inheritdoc />
    public async Task<List<SortPluginData<TMeta>>> InstallAsync(IEnumerable<string> shadowFiles)
    {
        var sortDataList = new List<SortPluginData<TMeta>>();
        foreach (var shadowFile in shadowFiles)
        {
            sortDataList.Add(new SortPluginData<TMeta>(
                await MetaDataHelper.ToMetaAsyncFromZip<TMeta>(shadowFile),
                shadowFile));
        }

        var res = DependencyChecker.DetermineLoadOrder(sortDataList);
        foreach (var data in res.Result)
        {
            await UnZip(data.Path, Path.Combine(BaseSdkConfig.PluginFolderPath, data.MetaData.DllName));
        }

        return res.Result;
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
}