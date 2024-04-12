using Windows.Storage;

namespace ShadowPluginLoader.WinUI.Helpers;

public class PluginStatusHelper
{
    private const string Container = "ShadowPluginLoader";

    public static bool Contains(string key)
    {
        var coreSettings =
                ApplicationData.Current.LocalSettings.CreateContainer(Container,
                    ApplicationDataCreateDisposition.Always);
        return coreSettings.Values.ContainsKey(key);
    }
    private static ApplicationDataCompositeValue GetPlugin(string key)
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
    private static void SetPlugin(string key, ApplicationDataCompositeValue value)
    {
        var coreSettings =
                ApplicationData.Current.LocalSettings.CreateContainer(Container,
                    ApplicationDataCreateDisposition.Always);
        coreSettings.Values[key] = value;
    }
    public static bool GetStatus(string key)
    {
        return (bool) GetPlugin(key)["enable"];
    }
    public static void SetStatus(string key, bool value)
    {
        var config = GetPlugin(key);
        config["enable"] = value;
        SetPlugin(key, config);
    }
}
