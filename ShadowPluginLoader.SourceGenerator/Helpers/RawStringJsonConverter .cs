using Newtonsoft.Json;

namespace ShadowPluginLoader.SourceGenerator.Helpers
{
    public class RawStringJsonConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            writer.WriteRawValue(value);
        }

        public override string ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (string)reader.Value!;
        }
    }
}
