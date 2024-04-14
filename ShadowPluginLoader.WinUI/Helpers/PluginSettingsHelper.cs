using Windows.Storage;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// Plugin Settings Helper
/// </summary>
public static class PluginSettingsHelper
{
    private const string Container = "ShadowPluginLoader";

    /// <summary>
    /// If Key Exists In The Local Settings
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <returns></returns>
    public static bool Contains(string key)
    {
        var coreSettings =
                ApplicationData.Current.LocalSettings.CreateContainer(Container,
                    ApplicationDataCreateDisposition.Always);
        return coreSettings.Values.ContainsKey(key);
    }
    /// <summary>
    /// Get Plugin Setting
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <returns></returns>
    private static ApplicationDataCompositeValue GetPluginSetting(string key)
    {
        ApplicationDataCompositeValue composite;
        if (Contains(key))
        {
            var coreSettings =
                ApplicationData.Current.LocalSettings.CreateContainer(Container,
                    ApplicationDataCreateDisposition.Always);
            composite = (ApplicationDataCompositeValue) coreSettings.Values[key];
        }
        else
        {
            composite = new ApplicationDataCompositeValue
            {
                ["enable"] = true,
                ["wait"] = 0, // 0:None, 1:update, 2:delete
                ["path"] = "",
            };
        }
        return composite;
    }
    /// <summary>
    /// Set Plugin Setting
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <param name="value">Plugin Setting</param>
    private static void SetPluginSetting(string key, ApplicationDataCompositeValue value)
    {
        var coreSettings =
                ApplicationData.Current.LocalSettings.CreateContainer(Container,
                    ApplicationDataCreateDisposition.Always);
        coreSettings.Values[key] = value;
    }
    /// <summary>
    /// Get Plugin IsEnabled
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <returns></returns>
    public static bool GetPluginIsEnabled(string key)
    {
        return (bool) GetPluginSetting(key)["enable"];
    }
    /// <summary>
    /// Set Plugin Enabled
    /// </summary>
    /// <param name="key">Plugin Id</param>
    /// <param name="value">Is Enabled</param>
    public static void SetPluginEnabled(string key, bool value)
    {
        var config = GetPluginSetting(key);
        config["enable"] = value;
        SetPluginSetting(key, config);
    }
}
