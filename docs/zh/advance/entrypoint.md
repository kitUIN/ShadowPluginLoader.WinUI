# 入口点

使用 `[EntryPoint]` 标记公开类，构建工具会把类名写入最终 `plugin.json`。显式填写 `Name`：当前特性的默认值为 `null`，工具不会自动替换成类名。

```csharp
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public class EmojiReader
{
}
```

## 生成格式

`[MainPlugin]` 对应顶层 `MainPlugin` 字符串与 `BuiltIn` 字段；普通入口点位于 `EntryPoints` 数组中。主类不再放入该数组。

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

## 运行时使用

主处理器加载程序集后调用 `BasePluginMetaData.ToBase(assembly)`，把 `MainPlugin` 转为 `Type`，把入口点转为 `PluginEntryPointType(Name, EntryPointType)` 数组。它不会按入口点名称自动填充你的自定义属性，也不会自动注册入口点服务。

可以在加载器的 `BeforeLoadPlugin(Type plugin, ExampleMetaData meta)` 中注册业务入口点，例如（补充 `System.Linq` 与 `DryIoc` 引用）：

```csharp
using System.Linq;
using DryIoc;

var reader = meta.EntryPoints.FirstOrDefault(x => x.Name == "EmojiReader");
if (reader?.EntryPointType is { } type)
{
    DiFactory.Services.Register(type, reuse: Reuse.Singleton);
}
```

入口点类必须存在于该插件程序集内，并为所需构造函数参数提供 DI 注册。更多生命周期钩子见[自定义加载逻辑](/zh/advance/customloadplugin)。
