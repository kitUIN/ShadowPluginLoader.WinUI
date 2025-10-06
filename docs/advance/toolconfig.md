# `Tool.Config.props` File

After depending on this project, a `Tool.Config.props` file will be generated in your project directory during the build process.

The file example is as follows:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <!-- Whether the current project is a PluginLoader -->
        <IsPluginLoader>false</IsPluginLoader>
        <!-- Whether the current project is a Plugin -->
        <IsPlugin>false</IsPlugin>
        <!-- Auto Pack Plugin When IsPlugin == True-->
        <AutoPluginPackage>true</AutoPluginPackage>
        <!-- Auto Generate I18N -->
        <AutoGenerateI18N>true</AutoGenerateI18N>
    </PropertyGroup>
</Project>
```

| Name | Type | Description |
| ------------- | :-----------: | ---- |
| `IsPluginLoader` | `bool` | Whether this project is a plugin loader project, see [IsPluginLoader](#IsPluginLoader) |
| `IsPlugin` | `bool` | Whether this project is a plugin project, see [IsPlugin](#IsPlugin) |
| `AutoPluginPackage` | `bool` | Whether to automatically package as plugin (only effective when `IsPlugin` is `true`), see [Plugin Packaging](/plugin/pack) |
| `AutoGenerateI18N` | `bool` | Whether to automatically generate I18N helper classes, see [I18N Internationalization](/advance/i18n) |

::: warning Note

Only one of `IsPluginLoader` and `IsPlugin` can be `true`.

:::

## IsPluginLoader

`IsPluginLoader` indicates whether this project is a plugin loader project.

If `true`:
- Export `[ExportMeta]` metadata classes as [metadata definition file](/advance/define)
- Package [metadata definition file](/advance/define) into `nuget` package's `build` folder and `buildTransitive` folder

## IsPlugin

`IsPlugin` indicates whether this project is a plugin project.

If `true`:
- Automatically generate `plugin.json` from `.csproj` file
- Allow using [plugin packaging](/plugin/pack) functionality