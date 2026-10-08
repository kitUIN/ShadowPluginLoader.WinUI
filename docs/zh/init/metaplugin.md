# 创建插件元数据类

加载器需要知道插件叫什么、版本是多少、依赖哪些插件。这些信息就是“元数据”。

在 SDK 中新建 `ExampleMetaData.cs`，继承 `BasePluginMetaData`。常用信息已经由基类提供，我们再加上作者和网址：

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

`[ExportMeta]` 告诉构建工具，用这个类生成 `plugin.d.json`。每个 SDK 选一个元数据类加上这个特性就可以了。

## 已经有哪些信息

下面这些属性可以直接使用：

| 属性 | 类型 | 用来做什么 |
| --- | --- | --- |
| `Id` / `Name` | `string` | 插件的唯一标识和名称 |
| `Version` | `NuGetVersion` | 插件版本，在 JSON 中写成字符串 |
| `SdkVersion` | `VersionRange` | 插件支持的 SDK 版本范围 |
| `Priority` | `int` | 加载优先级，默认 0，数字越小越早加载；依赖插件会先加载 |
| `Dependencies` | `PluginDependency[]` | 依赖哪些插件，以及它们的版本要求 |
| `DllName` | `string` | 插件程序集名称，不含 `.dll` |
| `BuiltIn` | `bool` | 是否为内置插件，可用 `[MainPlugin(BuiltIn = true)]` 设置 |
| `Raw` | `JsonElement` | 原始 JSON 信息 |
| `MainPlugin` | `Type` | 插件主类的类型 |
| `EntryPoints` | `PluginEntryPointType[]` | 插件提供的其他入口点 |

`SdkVersion` 一般不用自己填。比如 SDK 的程序集版本是 `1.3.1.0`，工具会填入 `[1.3, 1.4)`，表示支持 1.3 系列的 SDK。加载时会用这个范围检查 SDK 是否兼容。

## 添加自己的信息

像示例里的 `Authors` 和 `Url` 一样，添加属性并标上 `[Meta]` 就可以了。一般使用 `{ get; init; }`；数组和嵌套对象也能使用。可选属性记得给默认值，或者加上 `?`。

`Meta` 的常用设置如下：

| 设置 | 默认值 | 用法 |
| --- | --- | --- |
| `Required` | `true` | 是否必须填写 |
| `Exclude` | `false` | 设为 true 后，不写入元数据定义文件 |
| `Regex` | `null` | 用正则表达式检查字符串格式 |
| `AsString` | `false` | 在定义文件中把属性设为字符串类型 |
| `Converter` | `null` | 指定 `System.Text.Json` 转换器类型，需要无参构造函数 |

`AsString` 只决定定义文件中的类型。如果自定义类型需要从字符串转换，还要提供转换器；版本和依赖信息已经有内置支持。

插件如何填写这些信息，可以接着看[创建插件项目](/zh/plugin/create)。额外入口点的用法放在[入口点](/zh/advance/entrypoint)一节。
