# Create a Plugin Project

## Project and build configuration

Create a WinUI class library and reference the SDK published to your NuGet feed. The SDK version below matches the preceding tutorial.

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

## Metadata template

Create `plugin.json` in the project directory:

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}",
  "Authors": ["kitUIN"]
}
```

The build tool renders the Scriban template, validates it against `plugin.d.json`, and writes the final JSON to `{assembly name}/plugin.json` under the output directory. Deploy the output file, not the source template.

Template variables come from `PropertyGroup` nodes declared directly in the `.csproj`. The tool reads raw XML rather than evaluating MSBuild, so imported values, conditions, and `$(...)` expressions are not automatically evaluated for templates. The SDK NuGet package supplies the schema; for project references, the build target attempts to copy it from the referenced project's output.

Keep `Id`, `PackageId`, and the assembly name aligned: dependencies use package identifiers, while already loaded dependencies are recorded by `DllName`. Required fields are `Id`, `Name`, and `Version`. You can provide `Priority`, `SdkVersion`, and custom properties. If omitted, `SdkVersion` receives the schema's default range. The tool writes `DllName`, `MainPlugin`, `BuiltIn`, and `EntryPoints`.

## Main plugin class

The main class must be public, concrete, and derived from the SDK plugin base class. Declare one `[MainPlugin]` per plugin.

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

`[MainPlugin]` identifies the main class; `[CheckAutowired]` generates its constructor. `MetaData` is an instance property injected by the loader. Read it through `loader.GetPlugin(id)?.MetaData`.

## Plugin dependencies

Add an `ItemGroup` with `Label="Dependencies"`:

```xml
<ItemGroup Label="Dependencies">
  <PackageReference Include="ShadowExample.Plugin.Hello"
                    Version="1.1.6" Need="[1.0,2.0)" />
</ItemGroup>
```

`Version` selects the build-time NuGet package; `Need` is the allowed runtime version range. `[1.0,2.0)` includes 1.0 and excludes 2.0. The tool preserves explicitly supplied `Dependencies` in the template and appends this group's entries, so avoid duplicates.

Dependencies must already be loaded or be fed into the same pipeline. Ordinary `PackageReference` entries do not automatically become plugin dependencies. Runtime processing checks dependency versions and determines load order.

Add [controls](/plugin/control), [resource dictionaries](/plugin/resourcedictionary), [configuration](/plugin/config), or [localization](/advance/i18n), then [package the plugin](/plugin/pack).
