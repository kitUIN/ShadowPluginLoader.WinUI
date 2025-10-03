using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Converters;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Core.Tokens;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// Metadata helper for plugin configuration
/// </summary>
public static class MetaDataHelper
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// 
    /// </summary>
    public static JsonSerializerOptions? Options;

    /// <summary>
    /// 
    /// </summary>
    public static PropertyInfo[]? Properties { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    private static readonly Type TargetType = typeof(PluginEntryPointType);

    /// <summary>
    /// 
    /// </summary>
    private static readonly Type TargetTypeList = typeof(PluginEntryPointType[]);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMeta"></typeparam>
    /// <returns></returns>
    private static void GetAllProperties<TMeta>() where TMeta : AbstractPluginMetaData
    {
        if (Properties != null) return;
        var type = typeof(TMeta);
        var properties = new List<PropertyInfo>();
        while (type != null && type != typeof(object))
        {
            var props = type
                .GetProperties(BindingFlags.Public
                               | BindingFlags.Instance
                               | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == TargetType
                            || p.PropertyType == TargetTypeList);
            properties.AddRange(props);
            type = type.BaseType;
        }

        Properties = properties.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMeta"></typeparam>
    public static void Init<TMeta>() where TMeta : AbstractPluginMetaData
    {
        if (Options != null && Properties != null) return;
        lock (_lock)
        {
            GetAllProperties<TMeta>();
            if (Options != null) return;
            var converters = GetAllConverters(typeof(TMeta)).Distinct();
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new PluginDependencyJsonConverter());
            Options.Converters.Add(new NuGetVersionJsonConverter());
            Options.Converters.Add(new VersionRangeJsonConverter());
            foreach (var converter in converters)
            {
                if (!Options.Converters.Contains(converter))
                {
                    Options.Converters.Add(converter);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMeta"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    public static TMeta? ToMeta<TMeta>(string content) where TMeta : AbstractPluginMetaData
    {
        return JsonSerializer.Deserialize<TMeta>(content, Options);
    }

    /// <summary>
    /// Get and instantiate all Converters on properties (parent class + child class)
    /// </summary>
    private static IEnumerable<JsonConverter> GetAllConverters(Type type)
    {
        var converters = new List<JsonConverter>();

        while (type != null && type != typeof(object))
        {
            foreach (var prop in type.GetProperties(
                         BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var meta = prop.GetCustomAttribute<MetaAttribute>();
                if (meta?.Converter == null ||
                    !typeof(JsonConverter).IsAssignableFrom(meta.Converter)) continue;
                // Instantiate through Activator
                if (Activator.CreateInstance(meta.Converter) is JsonConverter instance)
                {
                    converters.Add(instance);
                }
            }

            type = type.BaseType; // Recursive parent class
        }

        return converters;
    }
}