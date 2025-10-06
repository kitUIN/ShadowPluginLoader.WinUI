# `Tool.Config.props`文件

依赖本项目之后,会在生成过程中生成一份`Tool.Config.props`文件到你的项目目录中

该文件示例如下:
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

| 名称       |      类型       | 说明 |
| ------------- | :-----------:  | ---- |
| `IsPluginLoader`      | `bool`  | 该项目是否是插件加载器项目,详见[IsPluginLoader](#IsPluginLoader) |
| `IsPlugin`      |   `bool`    |  该项目是否是插件项目,详见[IsPlugin](#IsPlugin) |
| `AutoPluginPackage` |   `bool`     |  是否自动打包为插件(仅在`IsPlugin`为`true`时有效),详见[插件打包](/zh/plugin/pack) |
| `AutoGenerateI18N` |   `bool`    |  是否自动生成I18N帮助类,详见[I18N国际化](/zh/advance/i18n) |

::: warning 注意

`IsPluginLoader`与`IsPlugin`只能有一个为`true`

:::

## IsPluginLoader

`IsPluginLoader`指明该项目是否是插件加载器项目

如果为`true`:
- 导出`[ExportMeta]`的元数据类为[元数据定义文件](/zh/advance/define)
- 把[元数据定义文件](/zh/advance/define)打包进`nuget`包的`build`文件夹与`buildTransitive`文件夹

## IsPlugin

`IsPlugin`指明该项目是否是插件项目

如果为`true`:
- 从`.csproj`文件中自动生成出`plugin.json`
- 允许使用[插件打包](/zh/plugin/pack)功能


