using ShadowObservableConfig.Attributes;
using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Config;

[ObservableConfig(FileName = "base_sdk")]
public partial class BaseSdkConfig
{
    [ObservableConfigProperty]
    private PluginUpgradeMethod upgradeMethod = PluginUpgradeMethod.Coverage;

    [ObservableConfigProperty]
    private bool closeInTaskBar;

    [ObservableConfigProperty]
    private bool closeInTaskBarRemember;

    [ObservableConfigProperty]
    private bool isDebug;
}