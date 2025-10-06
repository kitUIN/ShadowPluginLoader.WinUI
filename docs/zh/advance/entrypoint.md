# 注入点

使用`[EntryPoint]`特性标明类，该类将会修补进`plugin.json`

| 可配置项       |      类型      |  默认值 | 说明 |
| ------------- | :-----------: | ---- | ---- |
| `Name`      | `string?` | 类名 | 在`plugin.json`中的注入点名称 |


如果在`MetaData`中有对应Name的元数据项(类型需要为`PluginEntryPointType`),会自动载入

> 默认情况下会将`[MainPlugin]`指示的类名修补进`plugin.json`中,以便插件加载器快速识别类

### 示例
```csharp
namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public partial class EmojiReader
{
}
```

将自动修补进`plugin.json`

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