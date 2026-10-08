# 创建插件项目

SDK 准备好后，我们来写第一个插件。这里用 `ShadowExample.Plugin.Emoji` 作为例子。

## 新建项目

新建一个 WinUI 类库，引用前面生成的 SDK 包：

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

然后在项目根目录创建 `Tools.Config.props`，把 `IsPlugin` 设为 `true`。开启 `AutoPluginPackage` 后，每次构建都会顺便打包插件。

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

## 填写插件信息

在项目根目录新建 `plugin.json`：

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}",
  "Authors": ["kitUIN"]
}
```

双花括号里的名字对应 `.csproj` 中的属性。比如 `PackageId` 会被替换成 `ShadowExample.Plugin.Emoji`，以后改版本时也只需要修改项目里的 `Version`。

这些变量要直接写在 `.csproj` 的 `PropertyGroup` 中。模板读取的是这里填写的值，不能靠导入文件或 `$(...)` 表达式取值。

`Id`、`Name` 和 `Version` 是必填项。建议让 `Id`、`PackageId` 和程序集名称保持一致，方便其他插件声明依赖。SDK 版本范围、DLL 名称和主类等信息，工具会帮你补上。

构建后，输出目录的 `{程序集名称}/plugin.json` 就是填写完整的插件信息。发布时使用这个生成的文件。

## 写插件主类

新建 `EmojiPlugin.cs`，继承 SDK 中的 `PluginBase`：

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

`[MainPlugin]` 告诉加载器“从这个类开始”，每个插件加一处就够了。`[CheckAutowired]` 帮我们生成构造函数，`DisplayName` 则是插件的显示名称。

在插件内部，可以通过 `MetaData` 读取自己的信息；主程序可以通过 `loader.GetPlugin(id)?.MetaData` 读取。

## 如果还依赖其他插件

比如 Emoji 插件需要 Hello 插件，就在项目文件里加上：

```xml
<ItemGroup Label="Dependencies">
  <PackageReference Include="ShadowExample.Plugin.Hello"
                    Version="1.1.6" Need="[1.0,2.0)" />
</ItemGroup>
```

`Version` 是编译时引用的包版本，`Need` 是运行时允许的版本范围。这里的 `[1.0,2.0)` 表示大于等于 1.0、小于 2.0。

记得写上 `Label="Dependencies"`，工具才会把它当作插件依赖。也可以直接在 `plugin.json` 中填写 `Dependencies`，同一项选一种方式写就好。

加载时，可以先加载 Hello，也可以把两个插件一起交给加载器，它会安排好先后顺序。

到这里，一个最简单的插件就写好了。接下来可以[打包试用](/zh/plugin/pack)，或者继续添加[控件](/zh/plugin/control)、[资源字典](/zh/plugin/resourcedictionary)、[配置](/zh/plugin/config)和[多语言支持](/zh/advance/i18n)。
