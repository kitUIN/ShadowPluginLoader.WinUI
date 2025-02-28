using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ShadowPluginLoader.SourceGenerator.Helpers;

namespace ShadowPluginLoader.SourceGenerator.Models;
internal class Meta
{
    /// <summary>
    /// Is Required
    /// </summary>
    [JsonIgnore]
    public bool Required { get; set; } = true;

    /// <summary>
    /// Is Excluded
    /// </summary>
    [JsonIgnore]
    public bool Exclude { get; set; } = false;

    /// <summary>
    /// Mapping Project PropertyGroup Value
    /// </summary>
    public string? PropertyGroupName { get; set; }

    /// <summary>
    /// Using Regex To Validate
    /// </summary>
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? Regex { get; set; }
    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Nullable
    /// </summary>
    public bool Nullable { get; set; }

    /// <summary>
    /// Default
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(RawStringJsonConverter))]
    public string? DefaultValue { get; set; }

    public override string ToString()
    {
        var s1 = "";
        if (Regex != null)
        {
            s1 = $"""
                  , "Regex": "{Regex}"
                  """;
        }
        var s2 = "";
        if (DefaultValue != null)
        {
            s2 = $"""
                  , "DefaultValue": {DefaultValue}
                  """;
        }

        var nullable = Nullable ? "true" : "false";
        return $$"""
                  { "Type": "{{Type}}", "PropertyGroupName": "{{PropertyGroupName}}", "Nullable": {{nullable}}{{s1}}{{s2}} }
                  """;
    }
}
