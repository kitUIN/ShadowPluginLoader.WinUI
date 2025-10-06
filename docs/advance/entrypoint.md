# Entry Points

Use the `[EntryPoint]` attribute to mark a class, which will be patched into `plugin.json`.

| Configurable Item | Type | Default | Description |
| ------------- | :-----------: | ---- | ---- |
| `Name` | `string?` | Class name | Entry point name in `plugin.json` |

If there's a corresponding metadata item with the same Name in `MetaData` (type needs to be `PluginEntryPointType`), it will be automatically loaded.

> By default, the class name indicated by `[MainPlugin]` will be patched into `plugin.json` for quick class identification by the plugin loader.

### Example
```csharp
namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public partial class EmojiReader
{
}
```

Will be automatically patched into `plugin.json`:

```json
{
  "DllName": "ShadowExample.Plugin.Emoji",
  "Authors": [
    "kitUIN",
    "Hello"
  ],
  "Id": "ShadowExample.Plugin.Emoji",
  "Name": "emoji",
  "Version": "1.0.1.1",
  "SdkVersion": "1.2.6.0",
  "Dependencies": [],
  "EntryPoints": [
    {
      "Name": "MainPlugin",
      "Type": "ShadowExample.Plugin.Emoji.EmojiPlugin"
    },
    {  // [!code ++]
      "Name": "EmojiReader",  // [!code ++]
      "Type": "ShadowExample.Plugin.Emoji.EmojiReader"  // [!code ++]
    }  // [!code ++]
  ]
}
