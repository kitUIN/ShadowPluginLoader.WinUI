using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using ShadowPluginLoader.WinUI.Exceptions;

namespace ShadowPluginLoader.WinUI.Scanners
{
    /// <summary>
    /// IPluginScanSession
    /// </summary>
    /// <typeparam name="TAPlugin"></typeparam>
    /// <typeparam name="TMeta"></typeparam>
    public interface IPluginScanSession<TAPlugin, TMeta>
        where TAPlugin : AbstractPlugin<TMeta>
        where TMeta : AbstractPluginMetaData
    {
        /// <summary>
        /// Scan Plugin From Type<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(type);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="type">Plugin Type</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(Type? type);

        /// <summary>
        /// Scan Plugin From Type<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan&lt;TPlugin&gt;();
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        IPluginScanSession<TAPlugin, TMeta> Scan<TPlugin>();

        /// <summary>
        /// Scan Plugins From Type<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(types);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="types">Plugin Type List</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(IEnumerable<Type?> types);

        /// <summary>
        /// Scan Plugins From Type<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(uri);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="types">Plugin Type List</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(params Type?[] types);

        /// <summary>
        /// Scan Plugin From Optional Package<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(package);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="package">Optional Package</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(Package package);

        /// <summary>
        /// Scan Plugin From Plugin Path<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(pluginPath);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="pluginPath">Plugin Path</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(DirectoryInfo pluginPath);

        /// <summary>
        /// Scan Plugin From plugin.json<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(pluginJson);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="pluginJson">plugin.json</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(FileInfo pluginJson);

        /// <summary>
        /// Scan Plugin From Uri<br/>
        /// After Scan You Need Calling <see cref="FinishAsync"/>
        /// <example>
        /// <code>
        /// session.Scan(uri);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="uri">Uri</param>
        IPluginScanSession<TAPlugin, TMeta> Scan(Uri uri);

        /// <summary>
        /// Clear Scan History
        /// </summary>
        void ScanClear();

        /// <summary>
        /// Start Finish Scan
        /// <example>
        /// <code>
        /// session.Scan(pluginJson);
        /// // other session.Scan
        /// await session.FinishAsync();
        /// </code>
        /// </example>
        /// </summary>
        /// <exception cref="PluginScanException"></exception>
        Task<IEnumerable<string>> FinishAsync();
    }
}