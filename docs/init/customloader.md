# Create an SDK Project

The SDK is a WinUI class library shared by the host and plugins. It provides metadata, the plugin base class, the loader, and application APIs. This tutorial uses `ShadowExample.Core` throughout.

## Project configuration

Create a WinUI class library in Visual Studio and configure it as follows:

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

`3.1.9` is the current source project version. For unpublished source changes, build the corresponding packages into a local NuGet feed or use project references during development. Older packages may not contain these APIs. The SDK version `1.3.1` is an example; use the same version in subsequent plugin references.

`CopyLocalLockFileAssemblies` makes dependencies available during metadata export. `GeneratePackageOnBuild` produces the SDK NuGet package shared by plugins and the host. Add your own author, license, and repository metadata as needed.

## Mark the SDK project

Create this file in the project directory. The tool also copies a default file on the first build:

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

The build tool exports the metadata type marked `[ExportMeta]` into `plugin.d.json` and packages the definition and import properties under `build` / `buildTransitive`. Define the [metadata](/init/metaplugin), [plugin base class](/init/iplugin), and [loader](/init/customloaderclass) before building the SDK.

See [Tools.Config.props](/advance/toolconfig) for additional settings.
