using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ShadowPluginLoader.WinUI.Scanners;

/// <summary>
/// Plugin Scanner
/// </summary>
public interface IPluginScanner<TAPlugin, TMeta>
    where TAPlugin : AbstractPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{

    /// <summary>
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(type);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="type">Plugin Type</param>
    IPluginScanner<TAPlugin, TMeta> Scan(Type? type);

    /// <summary>
    /// Scan Plugin From Type<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan&lt;TPlugin&gt;();
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    IPluginScanner<TAPlugin, TMeta> Scan<TPlugin>();

    /// <summary>
    /// Scan Plugins From Type<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(types);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="types">Plugin Type List</param>
    IPluginScanner<TAPlugin, TMeta> Scan(IEnumerable<Type?> types);

    /// <summary>
    /// Scan Plugins From Type<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(uri);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="types">Plugin Type List</param>
    IPluginScanner<TAPlugin, TMeta> Scan(params Type?[] types);

    /// <summary>
    /// Scan Plugin From Optional Package<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(package);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="package">Optional Package</param>
    IPluginScanner<TAPlugin, TMeta> Scan(Package package);

    /// <summary>
    /// Scan Plugin From Plugin Path<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(pluginPath);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="pluginPath">Plugin Path</param>
    IPluginScanner<TAPlugin, TMeta> Scan(DirectoryInfo pluginPath);

    /// <summary>
    /// Scan Plugin From plugin.json<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(pluginJson);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="pluginJson">plugin.json</param>
    IPluginScanner<TAPlugin, TMeta> Scan(FileInfo pluginJson);

    /// <summary>
    /// Scan Plugin From Uri<br/>
    /// After Scan You Need Calling <see cref="FinishAsync"/>
    /// <example>
    /// <code>
    /// scanner.Scan(uri);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="uri">Uri</param>
    IPluginScanner<TAPlugin, TMeta> Scan(Uri uri);

    /// <summary>
    /// Clear Scan History
    /// </summary>
    void ScanClear();

    /// <summary>
    /// Start Finish Scan
    /// <example>
    /// <code>
    /// scanner.Scan(pluginJson);
    /// // other scanner.Scan
    /// await scanner.FinishAsync();
    /// </code>
    /// </example>
    /// </summary>
    /// <exception cref="PluginScanException"></exception>
    Task<IEnumerable<string>> FinishAsync();


    /// <summary>
    /// Checked for updates and removed plugins
    /// </summary>
    /// <returns></returns>
    Task CheckUpgradeAndRemoveAsync();
}