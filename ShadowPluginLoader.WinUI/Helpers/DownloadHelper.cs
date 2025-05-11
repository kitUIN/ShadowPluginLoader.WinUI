using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// DownloadHelper
/// </summary>
public static class DownloadHelper
{
    /// <summary>
    /// If local uri , return local uri. If http uri , download and return local uri
    /// </summary> 
    /// <returns></returns>
    public static async Task<string> DownloadFileAsync(string tempFolder,
        string downloadFile, ILogger? logger = null)
    {
        if (!downloadFile.StartsWith("http")) return downloadFile;
        var fileName = Path.GetFileName(downloadFile);
        var destinationPath = Path.Combine(tempFolder, fileName);
        if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
        logger?.Information("Start To Download File {httpPath} To {destinationPath}",
            downloadFile, destinationPath);
        using var client = new HttpClient();
        using var response = await client.GetAsync(new Uri(downloadFile), HttpCompletionOption.ResponseHeadersRead);
        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
        logger?.Information("Download File {httpPath} To {destinationPath} Success",
            downloadFile, destinationPath);
        return destinationPath;
    }
}