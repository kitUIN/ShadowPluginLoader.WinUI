# Entry Points

Mark public classes with `[EntryPoint]` to write their type names into the generated `plugin.json`. Set `Name` explicitly: its current default is `null`, and the tool does not substitute the class name.

```csharp
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public class EmojiReader
{
}
```

## Generated format

`[MainPlugin]` produces the top-level `MainPlugin` string and `BuiltIn` flag. Additional entry points appear in the `EntryPoints` array; the main class is not an array entry.

```json
{
  "Id": "ShadowExample.Plugin.Emoji",
  "Name": "Emoji",
  "Version": "1.1.0",
  "SdkVersion": "[1.3, 1.4)",
  "DllName": "ShadowExample.Plugin.Emoji",
  "BuiltIn": false,
  "Dependencies": [],
  "MainPlugin": "ShadowExample.Plugin.Emoji.EmojiPlugin",
  "EntryPoints": [
    {
      "Name": "EmojiReader",
      "Type": "ShadowExample.Plugin.Emoji.EmojiReader"
    }
  ]
}
```

## Runtime use

After loading the assembly, the main processor calls `BasePluginMetaData.ToBase(assembly)`. It resolves `MainPlugin` to a `Type` and entries to `PluginEntryPointType(Name, EntryPointType)` records. It does not fill custom properties by entry-point name or automatically register entry-point services.

Register application entry points in the loader's `BeforeLoadPlugin(Type plugin, ExampleMetaData meta)` hook, for example:

```csharp
using System.Linq;
using DryIoc;

var reader = meta.EntryPoints.FirstOrDefault(x => x.Name == "EmojiReader");
if (reader?.EntryPointType is { } type)
{
    DiFactory.Services.Register(type, reuse: Reuse.Singleton);
}
```

Entry-point classes must exist in the plugin assembly. Register any required constructor dependencies with DI. See [Custom Loading Logic](/advance/customloadplugin) for lifecycle hooks.
