using System.Collections.ObjectModel;
using ShadowObservableConfig.Attributes;

namespace ShadowPluginLoader.WinUI.Config;

[ObservableConfig(FileName = ".inner_sdk_config", FileExt = ".json")]
public partial class InnerSdkConfig
{
    /// <summary>
    /// Plan Remove Plugins
    /// </summary>
    [ObservableConfigProperty] private ObservableCollection<PlanRemoveData> _planRemove;

    /// <summary>
    /// Plan Upgrade Plugins
    /// </summary>
    [ObservableConfigProperty] private ObservableCollection<PlanUpgradeData> _planUpgrade;
}

[ObservableConfig]
public partial class PlanRemoveData
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    [ObservableConfigProperty] private string _id;

    /// <summary>
    /// Plugin Path
    /// </summary>
    [ObservableConfigProperty] private string _path;
}

[ObservableConfig]
public partial class PlanUpgradeData
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    [ObservableConfigProperty] private string _id;

    /// <summary>
    /// Plugin Path
    /// </summary>
    [ObservableConfigProperty] private string _targetPath;

    /// <summary>
    /// Plugin Zip Path
    /// </summary>
    [ObservableConfigProperty] private string _zipPath;
}