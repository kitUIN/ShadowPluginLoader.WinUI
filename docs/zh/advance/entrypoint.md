# 入口点

除了主类，插件有时还想提供其他类给主程序使用，比如一个负责读取内容的 `EmojiReader`。这时可以把它标记为入口点。

## 标记一个类

给公开类加上 `[EntryPoint]`，并用 `Name` 为它起一个名字：

```csharp
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[EntryPoint(Name = "EmojiReader")]
public class EmojiReader
{
}
```

`Name` 需要自己填写，后面就用这个名字查找入口点。

## 构建后会得到什么

工具会把入口点写进生成的 `plugin.json`。主类放在 `MainPlugin`，其他入口点放在 `EntryPoints`：

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

## 在加载器中使用

插件加载后，`MetaData.EntryPoints` 中就能拿到入口点的名称和 `Type`。比如找到 `EmojiReader` 后，把它注册到 DI 容器：

```csharp
using System.Linq;
using DryIoc;

var reader = meta.EntryPoints.FirstOrDefault(x => x.Name == "EmojiReader");
if (reader?.EntryPointType is { } type)
{
    DiFactory.Services.Register(type, reuse: Reuse.Singleton);
}
```

上面的代码可以放进加载器的 `BeforeLoadPlugin(Type plugin, ExampleMetaData meta)` 方法中。`Name` 用来查找，`EntryPointType` 就是找到的类。

如果这个类的构造函数还需要其他服务，也要提前注册好。加载器中可以在哪些地方加入自己的代码，见[自定义加载逻辑](/zh/advance/customloadplugin)。
