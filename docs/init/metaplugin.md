# Create Plugin Metadata

The loader needs to know a plugin's name, version, and dependencies. We call this information metadata.

Add `ExampleMetaData.cs` to the SDK and derive it from `BasePluginMetaData`. The base class provides the common fields; here we'll add authors and a website:

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

`[ExportMeta]` tells the build tool to generate `plugin.d.json` from this class. Choose one metadata class per SDK for this attribute.

## Fields you already have

These properties are ready to use:

| Property | Type | What it describes |
| --- | --- | --- |
| `Id` / `Name` | `string` | The plugin's unique identifier and name |
| `Version` | `NuGetVersion` | Plugin version, written as a JSON string |
| `SdkVersion` | `VersionRange` | Supported SDK versions |
| `Priority` | `int` | Defaults to 0; smaller numbers load earlier, with dependencies loaded first |
| `Dependencies` | `PluginDependency[]` | Required plugins and their version ranges |
| `DllName` | `string` | Assembly name without `.dll` |
| `BuiltIn` | `bool` | Whether the plugin is built in; set with `[MainPlugin(BuiltIn = true)]` |
| `Raw` | `JsonElement` | The original JSON information |
| `MainPlugin` | `Type` | The plugin's main class |
| `EntryPoints` | `PluginEntryPointType[]` | Additional entry points provided by the plugin |

You usually don't need to fill in `SdkVersion`. For an SDK assembly version of `1.3.1.0`, the tool supplies `[1.3, 1.4)`, allowing the 1.3 series. The loader checks this range for compatibility.

## Add your own fields

Add properties and mark them with `[Meta]`, just like `Authors` and `Url` above. Usually you'll use `{ get; init; }`. Arrays and nested objects work too. Give optional properties a default value or make them nullable with `?`.

Here are the common `Meta` settings:

| Setting | Default | What it does |
| --- | --- | --- |
| `Required` | `true` | Makes the field mandatory |
| `Exclude` | `false` | Leaves the field out of the metadata definition |
| `Regex` | `null` | Checks a string against a regular expression |
| `AsString` | `false` | Describes the field as a string in the definition |
| `Converter` | `null` | Sets a `System.Text.Json` converter type with a parameterless constructor |

`AsString` only changes the definition. If your custom type needs conversion from a string, provide a converter too. Versions and dependencies already have built-in support.

See [Create a Plugin](/plugin/create) to fill in these fields, or [Entry Points](/advance/entrypoint) to expose additional classes.
