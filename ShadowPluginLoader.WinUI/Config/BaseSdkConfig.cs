using Windows.Storage;
using ShadowObservableConfig.Attributes;
using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Config;

[ObservableConfig(FileName = "base_sdk", FileExt = ".json")]
public partial class BaseSdkConfig
{
    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private PluginUpgradeMethod _upgradeMethod = PluginUpgradeMethod.Coverage;

    /// <summary>
    /// 
    /// </summary>
    private readonly string _basePath = ApplicationData.Current.LocalFolder.Path;

    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private string _pluginFolder = "plugin";

    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private string _tempFolder = "temp";

    /// <summary>
    /// 
    /// </summary>
    public string PluginFolderPath => System.IO.Path.Combine(_basePath, _pluginFolder);

    /// <summary>
    /// 
    /// </summary>
    public string TempFolderPath => System.IO.Path.Combine(_basePath, _tempFolder);

    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private bool _closeInTaskBar;

    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private bool _closeInTaskBarRemember;

    /// <summary>
    /// 
    /// </summary>
    [ObservableConfigProperty] private bool _isDebug;
}