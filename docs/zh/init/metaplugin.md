# 创建插件元数据类

默认 `AbstractPluginLoader<TMeta, TAPlugin>`、`DiFactory.Init<TAPlugin, TMeta>()` 和主处理器要求 `TMeta` 继承 `BasePluginMetaData`。

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

`BasePluginMetaData` 继承 `AbstractPluginMetaData`，增加运行时解析的 `MainPlugin` 和 `EntryPoints`。仅继承 `AbstractPluginMetaData` 不满足默认加载器的泛型约束。`[ExportMeta]` 用于导出 `plugin.d.json`；一个 SDK 应提供一个导出的元数据类型。

## 内置属性

| 属性 | 类型 | 来源与用途 |
| --- | --- | --- |
| `Id` / `Name` | `string` | 插件标识 / 显示名称，模板中必填 |
| `Version` | `NuGetVersion` | 插件版本，JSON 中使用字符串 |
| `SdkVersion` | `VersionRange` | SDK 兼容范围；构建工具从定义文件补默认值 |
| `Priority` | `int` | 默认 0，数值小的优先，依赖关系优先于优先级 |
| `Dependencies` | `PluginDependency[]` | 依赖的 `Id` 与 `Need` 版本范围 |
| `DllName` | `string` | 构建工具写入的程序集名称，不含 `.dll` |
| `BuiltIn` | `bool` | 来自 `[MainPlugin(BuiltIn = true)]`，默认 false |
| `Raw` | `JsonElement` | 原始 JSON 的副本 |
| `MainPlugin` | `Type` | 加载程序集后解析的主类 |
| `EntryPoints` | `PluginEntryPointType[]` | 加载程序集后解析的额外入口点 |

例如 SDK 程序集版本为 `1.3.1.0` 时，工具默认生成 `SdkVersion: "[1.3, 1.4)"`。运行时检查的是元数据类型所在 SDK 程序集的版本。

## 自定义属性

使用可反序列化的属性（通常为 `{ get; init; }`）；数组和嵌套对象也可以作为元数据。可选字段应提供默认值或声明可空。`plugin.json` 的属性名需与元数据属性对应。

| `Meta` 配置 | 类型 | 默认值 | 作用 |
| --- | --- | --- | --- |
| `Required` | `bool` | `true` | 标记 Schema 必填字段 |
| `Exclude` | `bool` | `false` | 从导出的 Schema 中排除 |
| `Regex` | `string?` | `null` | Schema 字符串正则约束 |
| `AsString` | `bool` | `false` | 将 Schema 类型设为字符串 |
| `Converter` | `Type?` | `null` | 运行时注册的 `System.Text.Json` 转换器，须有无参构造函数 |
| `PropertyGroupName` | `string?` | `null` | 特性仍保留，但当前模板读取流程不使用它映射属性 |

用 `plugin.json` 中的 Scriban 模板引用项目属性，见[创建插件](/zh/plugin/create)。`AsString` 本身不提供运行时转换；版本和依赖已有内置转换器，自定义类型需自行处理。入口点通过 `MetaData.EntryPoints` 访问，见[入口点](/zh/advance/entrypoint)。
