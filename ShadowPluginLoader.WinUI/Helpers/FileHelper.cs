using Serilog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// FileHelper
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="targetFolder"></param>
    /// <param name="isDirectory"></param>
    /// <returns></returns>
    public static string GetName(string file, string targetFolder, bool isDirectory = false)
    {
        var fileNameExt = isDirectory ?  string.Empty: Path.GetExtension(file);
        var fileName = Path.GetFileNameWithoutExtension(file);
        var destinationPath = Path.Combine(targetFolder, $"{fileName}{fileNameExt}");
        var count = 0;
        while (isDirectory && Directory.Exists(destinationPath) || 
               !isDirectory && File.Exists(destinationPath))
        {
            destinationPath = Path.Combine(targetFolder, $"{fileName}{++count}{fileNameExt}");
        }

        if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
        return destinationPath;
    }


    /// <summary>
    /// If local uri , return local uri. If http uri , download and return local uri
    /// </summary> 
    /// <returns></returns>
    public static async Task<Uri> DownloadFileAsync(string tempFolder,
        Uri downloadFile, ILogger? logger = null)
    {
        if (!downloadFile.Scheme.StartsWith("http")) return downloadFile;
        var destinationPath = GetName(downloadFile.AbsolutePath, tempFolder);
        logger?.Information("Start To Download File {httpPath} To {destinationPath}",
            downloadFile, destinationPath);
        using var client = new HttpClient();
        using var response = await client.GetAsync(downloadFile, HttpCompletionOption.ResponseHeadersRead);
        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
        logger?.Information("Download File {httpPath} To {destinationPath} Success",
            downloadFile, destinationPath);
        return new Uri(destinationPath);
    }
}