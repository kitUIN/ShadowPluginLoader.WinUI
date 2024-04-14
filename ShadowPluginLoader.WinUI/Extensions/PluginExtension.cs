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
    /// <typeparam name="TIMeta">Your Custom Interface IMetaData Assignable To <see cref="Attribute"/>, <see cref="IPluginMetaData"/></typeparam>
    /// <typeparam name="TIPlugin">Your Custom Interface IPlugin Assignable To <see cref="IPlugin"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TIMeta? GetPluginMetaData<TIMeta,TIPlugin>() 
        where TIPlugin: IPlugin
        where TIMeta : Attribute, IPluginMetaData
    {
        return typeof(TIPlugin).GetPluginMetaData<TIMeta>();
    }
    /// <summary>
    /// Get PluginMetaData
    /// </summary>
    /// <param name="plugin">Plugin Type</param>
    /// <typeparam name="TIMeta">Your Custom Interface IMetaData Assignable To <see cref="Attribute"/>, <see cref="IPluginMetaData"/></typeparam>
    /// <returns>Your Custom Interface IMetaData</returns>
    public static TIMeta? GetPluginMetaData<TIMeta>(this Type plugin)
        where TIMeta : Attribute, IPluginMetaData
    {
        var meta = plugin.GetTypeInfo().GetCustomAttribute<TIMeta>();
        return meta;
    }
}