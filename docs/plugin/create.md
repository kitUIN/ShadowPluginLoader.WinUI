# Create a Plugin Project

With the SDK ready, let's write your first plugin. We'll call it `ShadowExample.Plugin.Emoji`.

## Create the project

Create a WinUI class library and reference the SDK package you built earlier:

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

Next, create `Tools.Config.props` in the project directory and set `IsPlugin` to `true`. Enabling `AutoPluginPackage` also packages the plugin whenever you build it.

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

## Fill in the plugin details

Create `plugin.json` in the project directory:

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}",
  "Authors": ["kitUIN"]
}
```

Names inside double braces refer to properties in the `.csproj`. For example, `PackageId` becomes `ShadowExample.Plugin.Emoji`. When you release a new version, you only need to change `Version` in the project file.

Declare these values directly in a `.csproj` `PropertyGroup`. The template reads those values; it doesn't evaluate imported properties or `$(...)` expressions.

`Id`, `Name`, and `Version` are required. Keep `Id`, `PackageId`, and the assembly name the same to make dependency references straightforward. The tool fills in the SDK version range, DLL name, main class, and other generated details.

After building, you'll find the completed metadata at `{assembly name}/plugin.json` in the output directory. Use this generated file when distributing the plugin.

## Write the main class

Create `EmojiPlugin.cs` and derive it from the SDK's `PluginBase`:

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

`[MainPlugin]` tells the loader where to start. Add it to one class per plugin. `[CheckAutowired]` generates the constructor, and `DisplayName` supplies the plugin's display name.

Inside the plugin, read its details through `MetaData`. In the app, use `loader.GetPlugin(id)?.MetaData`.

## Add dependencies if you need them

Suppose Emoji needs the Hello plugin. Add this to the project file:

```xml
<ItemGroup Label="Dependencies">
  <PackageReference Include="ShadowExample.Plugin.Hello"
                    Version="1.1.6" Need="[1.0,2.0)" />
</ItemGroup>
```

`Version` selects the package used for compilation. `Need` sets the allowed runtime versions: `[1.0,2.0)` means at least 1.0 and below 2.0.

Include `Label="Dependencies"` so the tool recognizes this as a plugin dependency. You can also write `Dependencies` directly in `plugin.json`; choose one place for each entry.

You can load Hello first or give both plugins to the loader together. It will put them in the right order.

Your basic plugin is ready. You can [package and try it](/plugin/pack), or add [controls](/plugin/control), [resource dictionaries](/plugin/resourcedictionary), [settings](/plugin/config), and [translations](/advance/i18n).
