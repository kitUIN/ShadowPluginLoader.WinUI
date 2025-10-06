# Plugin Packaging

## Prerequisites

Set `IsPlugin` and `AutoPluginPackage` to `true` in `Tool.Config.props`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <!-- Whether the current project is a PluginLoader -->
        <IsPluginLoader>false</IsPluginLoader>
        <!-- Whether the current project is a Plugin --> <!-- [!code focus] -->
        <IsPlugin>true</IsPlugin> <!-- [!code focus] -->
        <!-- Auto Pack Plugin When IsPlugin == True--> <!-- [!code focus] -->
        <AutoPluginPackage>true</AutoPluginPackage> <!-- [!code focus] -->
        <!-- Auto Generate I18N -->
        <AutoGenerateI18N>true</AutoGenerateI18N>
    </PropertyGroup>
</Project>
```

## Packaging

After build completion, it will automatically package as an `sdow` file.

Default path: `$(ProjectDir)Packages` (in the project folder's `Packages` folder)

::: tip Note
To modify default configuration, see: [Custom Packaging](/advance/custompluginbuild#自定义打包)
:::

### Exclude Files

Some files we don't want to package into the `sdow` file.

Create a new file `Plugin.Build.exclude` in the project folder.

For example:
```txt
Fluent/*
hello.*
```

Only supports:
- `?` wildcard for one character
- `*` wildcard for multiple characters

This way, the `Fluent` folder and files named `hello` in the output folder will not be packaged.

If you want to exclude a specific file in all subdirectories under a folder:

Use:
```txt
core/*text.txt 
```

Don't use:
```txt
core/**/text.txt 
```
