using System;
using System.IO;
using System.Text.Json;

namespace ShadowPluginLoader.WinUI.Extensions;

/// <summary>
/// PluginExtension That Can Get PluginMetaData
/// </summary>
public static class PluginExtension
{
    /// <summary>
    /// Get PluginMetaData
    /// </summary>
    /// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="AbstractPluginMetaData"/></typeparam>
    /// <typeparam name="TAPlugin">Your Custom Interface IPlugin Assignable To <see cref="AbstractPlugin{TMeta}"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TMeta? GetPluginMetaData<TMeta, TAPlugin>()
        where TAPlugin : AbstractPlugin<TMeta>
        where TMeta : AbstractPluginMetaData
    {
        return typeof(TAPlugin).GetPluginMetaData<TMeta>();
    }

    /// <summary>
    /// Get PluginMetaData
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="AbstractPluginMetaData"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TMeta? GetPluginMetaData<TMeta>(this Type plugin)
        where TMeta : AbstractPluginMetaData
    {
        var dir = plugin.Assembly.Location[..^".dll".Length];
        var metaPath = Path.Combine(dir, "Assets", "plugin.json");
        return !File.Exists(metaPath) ? null : JsonSerializer.Deserialize<TMeta>(File.ReadAllText(metaPath));
    }

}