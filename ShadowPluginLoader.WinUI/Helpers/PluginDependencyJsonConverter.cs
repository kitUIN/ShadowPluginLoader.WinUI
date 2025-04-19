using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// 
/// </summary>
public class PluginDependencyJsonConverter : JsonConverter<PluginDependency>
{
    /// <inheritdoc />
    public override PluginDependency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new PluginDependency(reader.GetString()!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PluginDependency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}