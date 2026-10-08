# Create Plugin Metadata

The default `AbstractPluginLoader<TMeta, TAPlugin>`, `DiFactory.Init<TAPlugin, TMeta>()`, and main processor require `TMeta` to derive from `BasePluginMetaData`.

```csharp [ExampleMetaData.cs]
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public record ExampleMetaData : BasePluginMetaData
{
    [Meta(Required = false)]
    public string[] Authors { get; init; } = [];

    [Meta(Required = false)]
    public string? Url { get; init; }
}
```

`BasePluginMetaData` extends `AbstractPluginMetaData` with runtime `MainPlugin` and `EntryPoints` resolution. Deriving only from `AbstractPluginMetaData` does not satisfy the default loader's generic constraint. `[ExportMeta]` exports `plugin.d.json`; provide one exported metadata type per SDK.

## Built-in properties

| Property | Type | Source and purpose |
| --- | --- | --- |
| `Id` / `Name` | `string` | Plugin identifier / display name; required in the template |
| `Version` | `NuGetVersion` | Plugin version, represented as a JSON string |
| `SdkVersion` | `VersionRange` | SDK compatibility range; the build tool applies the schema default |
| `Priority` | `int` | Defaults to 0; smaller values load earlier, subject to dependencies |
| `Dependencies` | `PluginDependency[]` | Dependency `Id` and `Need` version ranges |
| `DllName` | `string` | Assembly name without `.dll`, written by the build tool |
| `BuiltIn` | `bool` | From `[MainPlugin(BuiltIn = true)]`; defaults to false |
| `Raw` | `JsonElement` | A copy of the original JSON |
| `MainPlugin` | `Type` | Main class resolved after assembly loading |
| `EntryPoints` | `PluginEntryPointType[]` | Additional entry points resolved after assembly loading |

For SDK assembly version `1.3.1.0`, the tool generates the default `SdkVersion: "[1.3, 1.4)"`. Runtime checks use the version of the SDK assembly containing the metadata type.

## Custom properties

Use deserializable properties, normally `{ get; init; }`. Arrays and nested objects are supported. Give optional properties defaults or nullable types. JSON names must match the metadata properties.

| `Meta` option | Type | Default | Effect |
| --- | --- | --- | --- |
| `Required` | `bool` | `true` | Marks a required schema property |
| `Exclude` | `bool` | `false` | Removes the property from the exported schema |
| `Regex` | `string?` | `null` | Schema string pattern constraint |
| `AsString` | `bool` | `false` | Sets the schema type to string |
| `Converter` | `Type?` | `null` | Registers a runtime `System.Text.Json` converter with a parameterless constructor |
| `PropertyGroupName` | `string?` | `null` | Retained on the attribute, but not used for mapping by the current template reader |

Reference project properties with Scriban templates in `plugin.json`; see [Create a Plugin](/plugin/create). `AsString` does not implement runtime conversion. Versions and dependencies have built-in converters; custom types need their own handling. Access entry points through `MetaData.EntryPoints`, as described in [Entry Points](/advance/entrypoint).
