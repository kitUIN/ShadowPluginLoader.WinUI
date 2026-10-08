# 创建插件项目

## 项目与构建配置

创建 WinUI 类库并引用已发布到你的 NuGet 源的 SDK。下面的 SDK 版本与前文一致。

```xml [ShadowExample.Plugin.Emoji.csproj]
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
    <TargetPlatformMinVersion>10.0.19041.0</TargetPlatformMinVersion>
    <RootNamespace>ShadowExample.Plugin.Emoji</RootNamespace>
    <RuntimeIdentifiers>win-x86;win-x64;win-arm64</RuntimeIdentifiers>
    <UseWinUI>true</UseWinUI>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <PackageId>ShadowExample.Plugin.Emoji</PackageId>
    <PluginName>Emoji</PluginName>
    <Version>1.1.0</Version>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ShadowExample.Core" Version="1.3.1" />
  </ItemGroup>
</Project>
```

```xml [Tools.Config.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <IsPluginLoader>false</IsPluginLoader>
    <IsPlugin>true</IsPlugin>
    <AutoPluginPackage>true</AutoPluginPackage>
    <AutoGenerateI18N>true</AutoGenerateI18N>
  </PropertyGroup>
</Project>
```

## 元数据模板

在项目根目录创建 `plugin.json`：

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}",
  "Authors": ["kitUIN"]
}
```

构建工具用 Scriban 渲染模板，以 `plugin.d.json` 校验，并把最终 JSON 写入输出目录下的 `{程序集名称}/plugin.json`。源模板与最终文件不同；部署时使用输出文件。

模板变量来自 `.csproj` 中直接声明的 `PropertyGroup` 节点。当前工具读取原始 XML，不执行完整的 MSBuild 属性求值；不要依赖导入文件、条件求值或 `$(...)` 在模板中自动展开。SDK NuGet 包提供定义文件；项目引用时构建目标会尝试从被引用项目输出目录复制定义。

`Id` 建议与 `PackageId`、程序集名称保持一致，因为依赖声明使用包标识，而已加载依赖按 `DllName` 记录。必填字段为 `Id`、`Name`、`Version`；可填写 `Priority`、`SdkVersion` 和自定义字段。`SdkVersion` 未填写时由定义文件补默认范围。`DllName`、`MainPlugin`、`BuiltIn`、`EntryPoints` 由工具写入。

## 插件主类

主类必须是公开、非抽象的 SDK 插件基类派生类。每个插件只声明一个 `[MainPlugin]`。

```csharp [EmojiPlugin.cs]
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[MainPlugin]
[CheckAutowired]
public partial class EmojiPlugin : PluginBase
{
    public override string DisplayName => "EmojiPlugin";
}
```

`[MainPlugin]` 标记主类，`[CheckAutowired]` 生成构造函数，两者作用不同。`MetaData` 是实例属性，由加载器注入；可通过 `loader.GetPlugin(id)?.MetaData` 读取。

## 插件依赖

在项目中加入带 `Label="Dependencies"` 的 `ItemGroup`：

```xml
<ItemGroup Label="Dependencies">
  <PackageReference Include="ShadowExample.Plugin.Hello"
                    Version="1.1.6" Need="[1.0,2.0)" />
</ItemGroup>
```

`Version` 决定构建时引用的 NuGet 包，`Need` 是运行时允许的版本范围；`[1.0,2.0)` 包含 1.0，不包含 2.0。工具也保留模板中显式写入的 `Dependencies`，再追加这个分组的依赖，不要重复声明同一项。

被依赖插件必须已加载，或与当前插件一起投入同一个流水线。普通 `PackageReference` 不会自动成为插件依赖。运行时会检查依赖版本并安排加载顺序。

接下来可添加[控件](/zh/plugin/control)、[资源字典](/zh/plugin/resourcedictionary)、[配置](/zh/plugin/config)和[国际化](/zh/advance/i18n)，然后[打包](/zh/plugin/pack)。
