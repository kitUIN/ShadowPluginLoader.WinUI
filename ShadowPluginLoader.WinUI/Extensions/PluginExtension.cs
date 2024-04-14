using ShadowPluginLoader.WinUI.Interfaces;
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
    /// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="Attribute"/>, TIMeta</typeparam>
    /// <typeparam name="TIMeta">Your Custom Interface IMetaData Assignable To <see cref="IPluginMetaData"/></typeparam>
    /// <typeparam name="TIPlugin">Your Custom Interface IPlugin Assignable To <see cref="IPlugin"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TIMeta? GetPluginMetaData<TMeta, TIMeta, TIPlugin>() 
        where TIPlugin: IPlugin
        where TMeta : Attribute, TIMeta
        where TIMeta : IPluginMetaData
    {
        return typeof(TIPlugin).GetPluginMetaData<TMeta, TIMeta>();
    }

    /// <summary>
    /// Get PluginMetaData
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <typeparam name="TMeta">Your Custom Class MetaData Assignable To <see cref="Attribute"/>, TIMeta</typeparam>
    /// <typeparam name="TIMeta">Your Custom Interface IMetaData Assignable To <see cref="IPluginMetaData"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TIMeta? GetPluginMetaData<TMeta, TIMeta>(this Type plugin)
        where TMeta : Attribute, TIMeta
        where TIMeta : IPluginMetaData
    {
        var meta = plugin.GetTypeInfo().GetCustomAttribute<TMeta>();
        return meta;
    }
}