using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Base Plugin Installer
/// </summary>
public partial class BasePluginInstaller : IPluginInstaller
{
    /// <summary>
    /// Logger
    /// </summary>
    [Autowired]
    public ILogger Logger { get; }

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public bool Check(Uri path)
    {
        return true;
    }

    /// <inheritdoc />
    public Task<FileInfo> ScanAsync(Uri uri, string tempFolder, string targetFolder)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task UpgradeAsync(string pluginId, Uri uri, string tempFolder, string targetFolder)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Remove(string pluginId)
    {
        throw new NotImplementedException();
    }
}