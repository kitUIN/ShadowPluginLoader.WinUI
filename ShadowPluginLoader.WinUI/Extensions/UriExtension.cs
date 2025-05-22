using System;
using System.Linq;

namespace ShadowPluginLoader.WinUI.Extensions;

/// <summary>
/// 
/// </summary>
public static class UriExtension
{
    /// <summary>
    /// 
    /// </summary>
    public static string[] ZipType = [".zip", ".rar"];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static bool IsZip(this string uri)
    {
        return ZipType.Any(x => uri.EndsWith(x, StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static bool IsZip(this Uri uri)
    {
        return uri.AbsolutePath.IsZip();
    }
}