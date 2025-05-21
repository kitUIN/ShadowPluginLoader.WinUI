using Windows.Storage;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// Plugin Settings Helper
/// </summary>
public static class PluginSettingsHelper
{
    private const string Container = "ShadowPluginLoader";
    private const string EnabledPluginsKey = "EnabledPluginsKey";
    private const string UpgradePathKey = "UpgradePathKey";
    private const string UpgradeTargetPathKey = "UpgradeTargetPathKey";
    private const string RemovePathKey = "RemovePathKey";

    /// <summary>
    /// Get Plugin Setting
    /// </summary>
    /// <returns></returns>
    private static ApplicationDataCompositeValue GetPluginSetting(string key)
    {
        return SettingsHelper.Contains(Container, key)
            ? SettingsHelper.Get<ApplicationDataCompositeValue>(Container, key)!
            : new ApplicationDataCompositeValue();
    }

    /// <summary>
    /// Set Plugin Setting
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <param name="value">Plugin Setting</param>
    private static void SetPluginSetting(string key, ApplicationDataCompositeValue value)
    {
        SettingsHelper.Set(Container, key, value);
    }

    /// <summary>
    /// Get Plugin IsEnabled
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <returns></returns>
    public static bool GetPluginIsEnabled(string key)
    {
        var settings = GetPluginSetting(EnabledPluginsKey);
        if (settings.TryGetValue(key, out var setting)) return (bool)setting;
        return true;
    }

    /// <summary>
    /// Set Plugin Enabled
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <param name="value">Is Enabled</param>
    public static void SetPluginEnabled(string key, bool value)
    {
        var settings = GetPluginSetting(EnabledPluginsKey);
        settings[key] = value;
        SetPluginSetting(EnabledPluginsKey, settings);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetPluginPlanRemove(string key, string value)
    {
        var settings = GetPluginSetting(RemovePathKey);
        settings[key] = value;
        SetPluginSetting(RemovePathKey, settings);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ApplicationDataCompositeValue GetPluginRemovePaths()
    {
        return GetPluginSetting(RemovePathKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ApplicationDataCompositeValue GetPluginUpgradePaths()
    {
        return GetPluginSetting(UpgradePathKey);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ApplicationDataCompositeValue GetPluginUpgradeTargetPaths()
    {
        return GetPluginSetting(UpgradeTargetPathKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string? GetPluginUpgradeTargetPath(string key)
    {
        return GetPluginSetting(UpgradeTargetPathKey).TryGetValue(key, out var value) ? (string) value : null;
    }

    /// <summary>
    /// Set Plugin Upgrade File Path
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <param name="value">Path</param>
    /// <param name="targetPath">targetPath</param>
    public static void SetPluginUpgradePath(string key, string value, string targetPath)
    {
        var settings = GetPluginSetting(UpgradePathKey);
        settings[key] = value;
        SetPluginSetting(UpgradePathKey, settings);
        var settings2 = GetPluginSetting(UpgradeTargetPathKey);
        settings2[key] = targetPath;
        SetPluginSetting(UpgradeTargetPathKey, settings2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public static void DeleteUpgradePluginPath(string key)
    {
        var settings = GetPluginSetting(UpgradePathKey);
        settings.Remove(key);
        SetPluginSetting(UpgradePathKey, settings);
        var settings2 = GetPluginSetting(UpgradeTargetPathKey);
        settings2.Remove(key);
        SetPluginSetting(UpgradeTargetPathKey, settings2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public static void DeleteRemovePluginPath(string key)
    {
        var settings = GetPluginSetting(RemovePathKey);
        settings.Remove(key);
        SetPluginSetting(RemovePathKey, settings);
    }
}