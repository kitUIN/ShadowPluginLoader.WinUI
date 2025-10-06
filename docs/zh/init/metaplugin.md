# 创建插件元数据类

```csharp [ExampleMetaData.cs]
// 示例代码
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public record ExampleMetaData : AbstractPluginMetaData
{
    [Meta(Required = true, PropertyGroupName = "Author")]
    public string Author { get; init; }
}
```

## 导出元数据

`ExportMeta`特性指明这个类是需要导出的元数据

你的元数据类**必须**使用`ExportMeta`

会自动导出为`元数据定义文件`给后续的插件使用


## 继承

你的元数据类**必须**继承`AbstractPluginMetaData`
  
`AbstractPluginMetaData`是默认的插件元数据[AbstractPluginMetaData.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/AbstractPluginMetaData.cs)

## 额外的元数据项

在上文的示例中,我们新增了一个元数据项`Author`

```csharp
public string Author { get; init; }
```

- 所有的元数据项都需要使用属性访问器
  - `PluginEntryPointType` (`{ get;private set; }`)
  - 其他类型(`{ get; init; }`)
- 列表请使用`Array`
- 支持其他基本类型与实体类

## 元数据项的额外配置

```csharp
    [Meta(Required = true, PropertyGroupName = "Author")]
    public string Author { get; init; }
```
在上文中,我们使用了`Meta`特性

该特性用于配置我们的元数据项

| 可配置项       |      类型      |  默认值 | 说明 |
| ------------- | :-----------: | ---- | ---- |
| `Required`      | `bool` | `true` | 是否为必须项 |
| `Exclude`      |   `bool`   |   `false` | 是否忽略该属性,忽略后该属性不会被导出到define文件中 |
| `Regex` |   `string?`    |    `null` |  正则表达式,用于匹配该属性的值 |
| `PropertyGroupName` |   `string`    | 属性名称 |  元数据的对应的`MSBuild`名称,大小写敏感 |
| `Converter ` |   `Type`    | 自定义转换器 |  继承`JsonConverter`的自定义转换器 |
| `AsString ` |   `string`    | 保存是否为`string` |  序列化为`string` |
| ~~`Nullable`~~ |   ~~`bool`~~    |   ~~`false`~~ |  ~~是否允许该属性为`null`~~ 通过类名是否有问号自动判断 |



