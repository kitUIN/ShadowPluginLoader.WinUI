using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Scanners;

/// <summary>
/// Plugin Scanner
/// </summary>
public interface IPluginScanner<TAPlugin, TMeta>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <summary>
    /// StartScan
    /// </summary>
    /// <returns></returns>
    PluginScanSession<TAPlugin, TMeta> StartScan();

    /// <summary>
    /// FinishScanAsync
    /// </summary>
    Task<IEnumerable<string>> FinishScanAsync(Task<SortPluginData<TMeta>>[] scanTaskArray, Guid token);

    /// <summary>
    /// Check Sdk Version
    /// </summary>
    /// <exception cref="PluginScanException"></exception>
    void CheckSdkVersion(List<SortPluginData<TMeta>> metaList);
}