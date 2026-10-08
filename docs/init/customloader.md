# Create an SDK Project

Start with a class library that both your app and its plugins can reference. We'll call it the SDK. It will hold plugin metadata, the plugin base class, and any app features you want to make available to plugins.

## Create the library

In Visual Studio, create a WinUI class library named `ShadowExample.Core`. Open its project file and use this configuration as a starting point:

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

Two settings are useful here. `CopyLocalLockFileAssemblies` copies dependencies to the output directory so the tool can read your metadata. `GeneratePackageOnBuild` creates a NuGet package that the app and plugins can reference.

The example SDK version is `1.3.1`. You can choose your own version; just use the same one when referencing the SDK later.

## Mark it as an SDK project

Create `Tools.Config.props` in the project directory and set `IsPluginLoader` to `true`:

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

This tells the build tool to generate `plugin.d.json`, which describes the information plugins can provide. The file is included in the SDK's NuGet package for plugin projects to use.

Next, define your [plugin metadata](/init/metaplugin), [base class](/init/iplugin), and [loader](/init/customloaderclass). Once those are ready, build the SDK.

See [Tools.Config.props](/advance/toolconfig) for the other settings.
