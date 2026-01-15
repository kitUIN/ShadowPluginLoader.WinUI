using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// 
/// </summary>
/// <param name="TotalPercentage"></param>
/// <param name="TotalStatusValue"></param>
/// <param name="Step"></param>
/// <param name="SubPercentage"></param>
/// <param name="SubStatusValue"></param>
/// <param name="SubStep"></param>
public record PipelineProgress(
    double TotalPercentage = 0D,
    string TotalStatusValue = "",
    InstallPipelineStep Step = InstallPipelineStep.Feeding,
    double SubPercentage = 0D,
    string SubStatusValue = "",
    SubInstallPipelineStep SubStep = SubInstallPipelineStep.None
);