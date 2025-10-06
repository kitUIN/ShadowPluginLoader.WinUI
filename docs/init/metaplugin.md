# Create Plugin Metadata Class

```csharp [ExampleMetaData.cs]
// Example code
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

## Export Metadata

The `ExportMeta` attribute indicates that this class is metadata that needs to be exported.

Your metadata class **must** use `ExportMeta`.

It will automatically export as a `metadata definition file` for subsequent plugins to use.


## Inheritance

Your metadata class **must** inherit from `AbstractPluginMetaData`.

`AbstractPluginMetaData` is the default plugin metadata [AbstractPluginMetaData.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/AbstractPluginMetaData.cs)

## Additional Metadata Items

In the example above, we added a new metadata item `Author`.

```csharp
public string Author { get; init; }
```

- All metadata items need to use property accessors
  - `PluginEntryPointType` (`{ get;private set; }`)
  - Other types (`{ get; init; }`)
- Use `Array` for lists
- Supports other basic types and entity classes

## Additional Configuration for Metadata Items

```csharp
    [Meta(Required = true, PropertyGroupName = "Author")]
    public string Author { get; init; }
```

In the above, we used the `Meta` attribute.

This attribute is used to configure our metadata items.

| Configurable Item | Type | Default | Description |
| ------------- | :-----------: | ---- | ---- |
| `Required` | `bool` | `true` | Whether it's required |
| `Exclude` | `bool` | `false` | Whether to ignore this property, ignored properties won't be exported to define file |
| `Regex` | `string?` | `null` | Regular expression for matching the property value |
| `PropertyGroupName` | `string` | Property name | Corresponding `MSBuild` name for metadata, case sensitive |
| `Converter` | `Type` | Custom converter | Custom converter inheriting from `JsonConverter` |
| `AsString` | `string` | Whether to save as `string` | Serialize as `string` |
| ~~`Nullable`~~ | ~~`bool`~~ | ~~`false`~~ | ~~Whether to allow this property to be `null`~~ Automatically determined by whether class name has question mark |
