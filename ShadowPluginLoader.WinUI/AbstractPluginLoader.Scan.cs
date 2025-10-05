using ShadowPluginLoader.WinUI.Scanners;

namespace ShadowPluginLoader.WinUI;

public abstract partial class AbstractPluginLoader<TMeta, TAPlugin>
{
    /// <summary>
    /// PluginScanner
    /// </summary>
    protected IPluginScanner<TAPlugin, TMeta> PluginScanner { get; }

    /// <inheritdoc />
    public PluginScanSession<TAPlugin, TMeta> StartScan()
    {
        return PluginScanner.StartScan();
    }
}