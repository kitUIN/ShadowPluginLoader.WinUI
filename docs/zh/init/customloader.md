# 创建 SDK 项目

SDK 是宿主和插件共同引用的 WinUI 类库，提供元数据、插件基类、加载器和业务 API。以下统一使用 `ShadowExample.Core`。

## 项目配置

在 Visual Studio 中创建 WinUI 类库，配置项目文件：

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

`3.1.9` 对应当前源码项目版本。若使用尚未发布的源码，请先构建并将对应包放入本地 NuGet 源，或在开发时使用项目引用；不要假定旧版包已经包含这里的 API。SDK 的 `1.3.1` 是本教程示例版本，后续插件引用需保持一致。

`CopyLocalLockFileAssemblies` 确保导出元数据时能找到依赖程序集。`GeneratePackageOnBuild` 生成 SDK NuGet 包，供插件与宿主共同引用。可另外填写作者、许可证和仓库地址等 NuGet 信息。

## 标记 SDK 项目

在项目根目录创建以下文件（工具也会在首次构建时复制默认文件）：

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

构建时工具将导出带 `[ExportMeta]` 的元数据类，生成 `plugin.d.json`，并将定义和导入属性打入 SDK 包的 `build` / `buildTransitive` 目录。先完成[元数据类](/zh/init/metaplugin)、[插件基类](/zh/init/iplugin)和[加载器类](/zh/init/customloaderclass)，再构建 SDK。

更多配置见 [Tools.Config.props](/zh/advance/toolconfig)。
