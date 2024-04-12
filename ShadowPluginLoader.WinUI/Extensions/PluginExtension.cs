using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Reflection;

namespace ShadowPluginLoader.WinUI.Extensions;

public static class PluginExtension
{
    public static IM? GetPluginMetaData<IM,T>() 
        where T: IPlugin
        where IM : Attribute, IPluginMetaData
    {
        return typeof(T).GetPluginMetaData<IM>();
    }
    public static IM? GetPluginMetaData<IM>(this Type plugin)
        where IM : Attribute, IPluginMetaData
    {
        var meta = plugin.GetTypeInfo().GetCustomAttribute<IM>();
        return meta;
    }
}