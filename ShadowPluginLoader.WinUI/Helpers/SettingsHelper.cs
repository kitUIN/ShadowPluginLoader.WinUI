using Windows.Storage;
using DryIoc;
using Serilog;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// Settings Helper
/// </summary>
public static class SettingsHelper
{
    /// <summary>
    /// Determines whether a specific key exists in the specified application data container.
    /// </summary>
    /// <param name="key">The key to look for in the container.</param>
    /// <param name="container">The name of the container where the key is being checked.</param>
    /// <returns>True if the key exists in the container; otherwise, false.</returns>
    public static bool Contains(string container, string key)
    {
        var coreSettings =
            ApplicationData.Current.LocalSettings.CreateContainer(container,
                ApplicationDataCreateDisposition.Always);
        return coreSettings.Values.ContainsKey(key);
    }

    /// <summary>
    /// Retrieves the value associated with the specified key in the specified application data container.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="container">The name of the container where the key is located.</param>
    /// <param name="key">The key associated with the value to retrieve.</param>
    /// <returns>The value of type <typeparamref name="T"/> if the key exists; otherwise, the default value of <typeparamref name="T"/>.</returns>
    public static T? Get<T>(string container, string key)
    {
        var coreSettings =
            ApplicationData.Current.LocalSettings.CreateContainer(container,
                ApplicationDataCreateDisposition.Always);
        if (!coreSettings.Values.TryGetValue(key, out var value))
        {
            return default;
        }

        return (T)value;
    }

    /// <summary>
    /// Sets a value for a specific key in the specified application data container.
    /// </summary>
    /// <param name="container">The name of the container where the key-value pair is stored.</param>
    /// <param name="key">The key associated with the value to be set.</param>
    /// <param name="value">The value to set for the specified key.</param>
    public static void Set(string container, string key, object value)
    {
        var coreSettings =
            ApplicationData.Current.LocalSettings.CreateContainer(container,
                ApplicationDataCreateDisposition.Always);
        coreSettings.Values[key] = value;
        Log.Debug("Container: {container} | [{key}] {value}", container, key, value);
    }
}