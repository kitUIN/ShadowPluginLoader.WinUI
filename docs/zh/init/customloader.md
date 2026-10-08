# 创建 SDK 项目

先来建一个主程序和插件都能引用的类库。我们把它叫作 SDK，用来放插件信息、插件基类，以及主程序想提供给插件的功能。

## 新建类库

在 Visual Studio 中创建一个 WinUI 类库，命名为 `ShadowExample.Core`。打开项目文件，参考下面的配置：

```xml [ShadowExample.Core.csproj]
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
    <TargetPlatformMinVersion>10.0.19041.0</TargetPlatformMinVersion>
    <RootNamespace>ShadowExample.Core</RootNamespace>
    <RuntimeIdentifiers>win-x86;win-x64;win-arm64</RuntimeIdentifiers>
    <UseWinUI>true</UseWinUI>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PackageId>ShadowExample.Core</PackageId>
    <Version>1.3.1</Version>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageOutputPath>../NugetPackages</PackageOutputPath>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ShadowPluginLoader.WinUI" Version="3.1.9" />
  </ItemGroup>
</Project>
```

这里有两个配置值得留意：`CopyLocalLockFileAssemblies` 会把依赖库一起复制到输出目录，方便工具读取元数据；`GeneratePackageOnBuild` 会在构建时生成 NuGet 包，之后主程序和插件就能引用它。

示例中 SDK 的版本是 `1.3.1`。你可以改成自己的版本，后面引用时对应上就好。

## 告诉工具这是 SDK 项目

在项目根目录新建 `Tools.Config.props`，把 `IsPluginLoader` 设为 `true`：

```xml [Tools.Config.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <IsPluginLoader>true</IsPluginLoader>
    <IsPlugin>false</IsPlugin>
    <AutoPluginPackage>false</AutoPluginPackage>
    <AutoGenerateI18N>true</AutoGenerateI18N>
  </PropertyGroup>
</Project>
```

这个开关会让工具在构建时生成 `plugin.d.json`，用来描述插件可以填写哪些信息。它也会随 SDK 一起打进 NuGet 包，供插件项目使用。

接下来先[定义插件元数据](/zh/init/metaplugin)，再写[插件基类](/zh/init/iplugin)和[加载器](/zh/init/customloaderclass)。这些代码准备好后，就可以构建 SDK 了。

其他开关的用途可以查看 [Tools.Config.props](/zh/advance/toolconfig)。
