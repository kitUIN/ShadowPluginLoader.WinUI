using Windows.Storage;
using ShadowObservableConfig.Attributes;
using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Config;

[ObservableConfig(FileName = "base_sdk")]
public partial class BaseSdkConfig
{

    [ObservableConfigProperty] private PluginUpgradeMethod upgradeMethod = PluginUpgradeMethod.Coverage;

    private string basePath = ApplicationData.Current.LocalFolder.Path;

    [ObservableConfigProperty] private string pluginFolder = "plugin";

    [ObservableConfigProperty] private string tempFolder = "temp";

    public string PluginFolderPath => System.IO.Path.Combine(basePath, pluginFolder);
    public string TempFolderPath => System.IO.Path.Combine(basePath, tempFolder);

    [ObservableConfigProperty] private bool closeInTaskBar;

    [ObservableConfigProperty] private bool closeInTaskBarRemember;

    [ObservableConfigProperty] private bool isDebug;
}