using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Reflection;

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
    /// <typeparam name="TAPlugin">Your Custom Interface IPlugin Assignable To <see cref="APlugin"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TMeta? GetPluginMetaData<TMeta, TAPlugin>() 
        where TAPlugin: APlugin
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
        var meta = plugin.GetTypeInfo().GetCustomAttribute<TMeta>();
        return meta;
    }
}