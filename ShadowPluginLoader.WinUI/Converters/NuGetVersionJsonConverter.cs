using NuGet.Versioning;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShadowPluginLoader.WinUI.Converters;

public class NuGetVersionJsonConverter : JsonConverter<NuGetVersion>
{
    public override NuGetVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new NuGetVersion(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, NuGetVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}