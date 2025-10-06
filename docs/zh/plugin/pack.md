# 插件打包

## 前置要求

`Tool.Config.props`中`IsPlugin`与`AutoPluginPackage`设置为`true`

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

## 打包

在生成结束后会自动打包为`zip`文件

默认路径为:`$(ProjectDir)Packages`(项目文件夹的`Packages`文件夹内)

::: tip 说明
如果要修改默认配置,请查阅: [自定义打包](/zh/advance/custompluginbuild#自定义打包)
:::

### 排除文件

有些不需要的文件我们不想打包进`zip`文件中

在项目文件夹新建文件`Plugin.Build.exclude`

例如:
```txt
Fluent/*
hello.*
```

仅能使用
- `?` 通配一个字符
- `*` 通配多个字符

这样在输出文件夹内的`Fluent`文件夹和名为`hello`的文件都不会被打包

如果你想排除文件夹下所有子目录中的某个文件

应当使用

```txt
core/*text.txt 
```

不要使用

```txt
core/**/text.txt 
```