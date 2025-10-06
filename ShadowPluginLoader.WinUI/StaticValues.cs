using Windows.Storage;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// 
/// </summary>
public static class StaticValues
{
    /// <summary>
    /// 
    /// </summary>
    public static bool RemoveChecked { get; set; }


    /// <summary>
    /// 
    /// </summary>
    public static bool UpgradeChecked { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public static string BaseFolder { get; } = ApplicationData.Current.LocalFolder.Path;
}