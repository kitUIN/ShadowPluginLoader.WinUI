# Entry Points

Sometimes a plugin needs to expose more than its main class. For example, it might provide an `EmojiReader` that the app can use to read content. Mark that class as an entry point.

## Mark a class

Add `[EntryPoint]` to a public class and give it a `Name`:

```csharp
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public class EmojiReader
{
}
```

Set `Name` yourself. You'll use it to find the entry point later.

## What gets generated?

The tool writes entry points into the generated `plugin.json`. The main class goes in `MainPlugin`, and additional classes go in `EntryPoints`:

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

## Use it in the loader

After loading the plugin, `MetaData.EntryPoints` gives you each entry point's name and `Type`. For example, find `EmojiReader` and register it with DI:

```csharp
using System.Linq;
using DryIoc;

var reader = meta.EntryPoints.FirstOrDefault(x => x.Name == "EmojiReader");
if (reader?.EntryPointType is { } type)
{
    DiFactory.Services.Register(type, reuse: Reuse.Singleton);
}
```

Put this code in the loader's `BeforeLoadPlugin(Type plugin, ExampleMetaData meta)` method. Use `Name` to find the entry and `EntryPointType` to get its class.

If the class's constructor needs other services, register those too. See [Custom Loading Logic](/advance/customloadplugin) for places to add your own code during loading.
