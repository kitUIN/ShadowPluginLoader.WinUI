using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Scanners;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Config;

namespace ShadowPluginLoader.WinUI.Installer;

/// <inheritdoc />
public class ZipPluginInstaller<TAPlugin, TMeta> : IPluginInstaller
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly IDependencyChecker<TMeta> DependencyChecker;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IPluginScanner<TAPlugin, TMeta> PluginScanner;

    /// <summary>
    /// 
    /// </summary>
    protected readonly BaseSdkConfig BaseSdkConfig;

    /// <summary>
    /// 
    /// </summary>
    public ZipPluginInstaller(IDependencyChecker<TMeta> dependencyChecker,
        IPluginScanner<TAPlugin, TMeta> pluginScanner, BaseSdkConfig baseSdkConfig)
    {
        DependencyChecker = dependencyChecker;
        PluginScanner = pluginScanner;
        BaseSdkConfig = baseSdkConfig;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> InstallAsync(IEnumerable<string> shadowFiles)
    {
        var sortDataList = new List<SortPluginData<TMeta>>();
        foreach (var shadowFile in shadowFiles)
        {
            sortDataList.Add(new SortPluginData<TMeta>(
                await MetaDataHelper.ToMetaAsyncFromZip<TMeta>(shadowFile),
                shadowFile));
        }

        var res = DependencyChecker.DetermineLoadOrder(sortDataList);
        var session = PluginScanner.StartScan();
        foreach (var data in res.Result)
        {
            var path = Path.Combine(BaseSdkConfig.PluginFolderPath, data.MetaData.DllName);
            await UnZip(data.Path, path);
            session.Scan(new Uri(Path.Combine(path, data.MetaData.DllName, "Assets", "plugin.json")));
        }

        return await session.FinishAsync();
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