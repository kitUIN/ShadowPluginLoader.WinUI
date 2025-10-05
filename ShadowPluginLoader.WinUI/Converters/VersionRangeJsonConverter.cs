using NuGet.Versioning;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace ShadowPluginLoader.WinUI.Converters;

/// <summary>
/// VersionRangeJsonConverter
/// </summary>
public class VersionRangeJsonConverter : JsonConverter<VersionRange>
{
    /// <inheritdoc />
    public override VersionRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return VersionRange.Parse(reader.GetString()!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, VersionRange value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}