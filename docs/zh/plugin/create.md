# 创建插件项目

我们首先需要在`VS`中新建一个`WinUI`类库

![winui](/init/winui.png)

## 编写默认元数据

默认提供了五个可填写的元数据项
- `Id`:插件Id
- `Name`:插件名称
- `Version`:插件版本
- `Priority `:加载顺序,越小越早加载
- `Dependencies`:插件依赖
- `SdkVersion`: (自动)SDK版本
- `BuiltIn`: (自动)是否为内置插件
- `DllName`: (自动)DLL文件名
- `MainPlugin`: (自动)主插件类型
- `EntryPoints`: (自动)入口点



元数据项(除了`Dependencies`)均在`plugin.json`中设置

```xml [ShadowExample.Plugin.Emoji.csproj]
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net6.0-windows10.0.19041.0</TargetFramework>
        <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
        <RootNamespace>ShadowExample.Plugin.Emoji</RootNamespace>
        <RuntimeIdentifiers>win10-x86;win10-x64;win10-arm64</RuntimeIdentifiers>
        <UseWinUI>true</UseWinUI>
        <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
        <NoWarn>MSB3277</NoWarn>
        <!-- Nuget -->
        <PackageId>ShadowExample.Plugin.Emoji</PackageId>
        <PluginName>Emoji插件</PluginName>
        <Version>1.0.0.12</Version>
        <!-- PluginMeta --> <!-- [!code ++] -->
    </PropertyGroup>
</Project>
```

需要在根目录新建`plugin.json`

可以使用`{{ variable in PropertyGroup }}`获取`.csproj`中的变量

`Id`必须与`PackageId`对应

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}"
}
```

::: tip 说明

在`<PluginMeta>`中可以使用`MSBuild`的属性变量
- [MSBuild 保留属性和已知属性](https://learn.microsoft.com/zh-cn/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022)
- [常用的 MSBuild 项目属性](https://learn.microsoft.com/zh-cn/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022)
:::

元数据项`Dependencies`用于指明插件的`依赖插件`,让插件在`依赖插件`加载后再加载

与常规元数据项不同,需要在`.csproj`中的`<ItemGroup>`里设置

同时他作为项目的`nuget`包

- 必须指明`Label="Dependencies"`
- 内部必须指明`Need="(1.0,2.0)"`要求的版本
```xml [ShadowExample.Plugin.Emoji.csproj]
<ItemGroup Label="Dependencies">
    <PackageReference Include="ShadowExample.Plugin.Hello" Version="1.2.1.2"  Need="(1.0,2.0)"  />
    <PackageReference Include="ShadowExample.Plugin.World" Version="1.3.0.0"  Need="(1.0,2.0)"  />
</ItemGroup>
```

## 插件主类

插件主类需要继承插件加载器中的自定义的插件基类

并且必须使用`[MainPlugin]`特性指明插件主类,这会自动生成对应的插件元信息

```csharp [LocalPlugin.cs]
// 示例的插件主类
namespace ShadowViewer.Plugin.Local;

[MainPlugin]
public partial class LocalPlugin : PluginBase
{
}
```

::: tip
你可以直接使用`LocalPlugin.MetaData`来获取插件的元数据信息
:::

## 后续编写

接下来,你可以选择覆写插件基类中的默认方法

或者
- [使用I18N国际化](/zh/advance/i18n)
- [自定义WinUI控件](/zh/plugin/control)
- [自定义资源字典](/zh/plugin/resourcedictionary)
- [自定义资源文件](/zh/plugin/assets)