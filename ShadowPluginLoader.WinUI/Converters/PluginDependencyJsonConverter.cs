using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowPluginLoader.WinUI.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 
/// </summary>
public class PluginDependencyJsonConverter : JsonConverter<PluginDependency>
{
    public override PluginDependency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject");

        var id = "";
        var need = "";

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "Id":
                        id = reader.TokenType == JsonTokenType.String ? reader.GetString() ?? "" : "";
                        break;
                    case "Need":
                        need = reader.TokenType == JsonTokenType.String ? reader.GetString() ?? "" : "";
                        break;

                    default:
                        reader.Skip(); // 跳过未知字段
                        break;
                }
            }
        }

        return new PluginDependency(id, need);
    }

    public override void Write(Utf8JsonWriter writer, PluginDependency value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Id", value.Id);
        writer.WriteString("Need", value.Need.ToString());
        writer.WriteEndObject();
    }
}
