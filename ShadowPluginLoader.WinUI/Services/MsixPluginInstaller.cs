using System;
using ShadowPluginLoader.Attributes;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// Msix Plugin Installer
/// </summary>
[CheckAutowired]
public partial class MsixPluginInstaller : BasePluginInstaller
{
    /// <inheritdoc />
    public override int Priority => 1;

    /// <inheritdoc />
    public override string Identify => "Msix";

    /// <inheritdoc />
    public override bool Check(Uri path)
    {
        return path.AbsolutePath.EndsWith(".msix", StringComparison.OrdinalIgnoreCase);
    }
}