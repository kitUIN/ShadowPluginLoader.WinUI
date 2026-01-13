using System;
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
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Enums;

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
    public async Task<List<SortPluginData<TMeta>>> InstallAsync(IEnumerable<string> shadowFiles,
        IProgress<InstallProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        var files = shadowFiles.ToList();
        var sortDataList = new List<SortPluginData<TMeta>>();

        // --- 阶段 1: 下载与解析 (占比 0% - 40%) ---
        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var (fileBaseOffset, fileWeight) = ProgressHelper.CreateSubProgressBegin(
                i, file.Length, 40);
            var currentPath = file;
            if (file.StartsWith("http"))
            {
                var tempFile = Path.Combine(BaseSdkConfig.TempFolderPath, Path.GetFileName(file));

                // 创建一个子进度汇报器，将 0-1 的下载进度映射到全局进度
                var downloadProgress = new Progress<double>(p =>
                {
                    progress?.Report(new InstallProgress($"{Path.GetFileName(file)} ({p:P2})",
                        Percentage: fileBaseOffset + (p * fileWeight), InstallProgressStep.Downloading));
                });

                await BaseHttpHelper.Instance.SaveFileAsync(file, tempFile, progress: downloadProgress,
                    cancellationToken: cancellationToken);
                currentPath = tempFile;
            }
            else
            {
                progress?.Report(new InstallProgress($"{Path.GetFileName(file)}",
                    Percentage: fileBaseOffset + fileWeight, InstallProgressStep.Downloading));
            }

            sortDataList.Add(new SortPluginData<TMeta>(await MetaDataHelper.ToMetaAsyncFromZip<TMeta>(currentPath),
                currentPath));
        }

        // --- 阶段 2: 排序与解压 (占比 40% - 90%) ---
        var res = DependencyChecker.DetermineLoadOrder(sortDataList);
        var sortedList = res.Result;

        for (var i = 0; i < sortedList.Count; i++)
        {
            var data = sortedList[i];
            var (fileBaseOffset, fileWeight) = ProgressHelper.CreateSubProgressBegin(
                i, sortedList.Count, 50, 40);
            var unzipProgress = new Progress<ProgressReport>(r =>
            {
                var p = r.PercentComplete ?? 0D;
                progress?.Report(new InstallProgress($"{data.MetaData.DllName} ({p:P2})",
                    fileBaseOffset + (p * fileWeight), InstallProgressStep.UnZipping));
            });
            await UnZip(data.Path, Path.Combine(BaseSdkConfig.PluginFolderPath, data.MetaData.DllName), unzipProgress,
                cancellationToken);
        }

        return sortedList;
    }


    /// <summary>
    /// UnZip
    /// </summary>
    protected virtual async Task<string> UnZip(string zipPath, string outputPath,
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
        await archive.WriteToDirectoryAsync(outputPath, new ExtractionOptions
        {
            ExtractFullPath = true,
            Overwrite = true,
        }, progress, cancellationToken);
        return outputPath;
    }
}