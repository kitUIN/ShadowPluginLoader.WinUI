using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Exceptions;
using ShadowPluginLoader.WinUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// Metadata helper for plugin configuration
/// </summary>
public static class MetaDataHelper
{
    private static readonly object _lock = new();

    public static JsonSerializerOptions? Options;


    public static void Init<TMeta>() where TMeta : AbstractPluginMetaData
    {
        if (Options != null) return;
        lock (_lock)
        {
            if (Options != null) return;
            var converters = GetAllConverters(typeof(TMeta)).Distinct();
            Options = new JsonSerializerOptions();
            foreach (var converter in converters)
                Options.Converters.Add(converter);
        }
    }

    public static TMeta? ToMeta<TMeta>(string content) where TMeta : AbstractPluginMetaData
    {
        if (Options == null) Init<TMeta>();
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
                if (meta?.Converter != null &&
                    typeof(JsonConverter).IsAssignableFrom(meta.Converter))
                {
                    // Instantiate through Activator
                    if (Activator.CreateInstance(meta.Converter) is JsonConverter instance)
                    {
                        converters.Add(instance);
                    }
                }
            }

            type = type.BaseType; // Recursive parent class
        }

        return converters;
    }
}