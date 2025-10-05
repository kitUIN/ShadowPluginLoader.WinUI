using System.Collections.ObjectModel;
using ShadowObservableConfig.Attributes;

namespace ShadowPluginLoader.WinUI.Config;

[ObservableConfig(FileName = ".inner_sdk_config")]
public partial class InnerSdkConfig
{
    [ObservableConfigProperty]
    private ObservableCollection<PlanRemoveData> planRemove;

    [ObservableConfigProperty]
    private ObservableCollection<PlanUpgradeData> planUpgrade;
}

[ObservableConfig]
public partial class PlanRemoveData
{
    [ObservableConfigProperty]
    private string id;

    [ObservableConfigProperty]
    private string path;
}

[ObservableConfig]
public partial class PlanUpgradeData
{
    [ObservableConfigProperty]
    private string id;

    [ObservableConfigProperty]
    private string targetPath;

    [ObservableConfigProperty]
    private string zipPath;
}