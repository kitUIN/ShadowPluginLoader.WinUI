using ShadowPluginLoader.WinUI.Enums;

namespace ShadowPluginLoader.WinUI.Models;


/// <summary>
/// 
/// </summary>
public record InstallProgress(string StatusName, double Percentage, InstallProgressStep Step)
{
    /// <summary>
    /// PercentageString
    /// </summary>
    public string PercentageString => $"{Percentage / 100.0:P2}";
}