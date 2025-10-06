# Create Plugin Project

We first need to create a new `WinUI` class library in `VS`.

![winui](/init/winui.png)

## Write Default Metadata

Five fillable metadata items are provided by default:
- `Id`: Plugin ID
- `Name`: Plugin name
- `Version`: Plugin version
- `Priority`: Loading order, smaller values load earlier
- `Dependencies`: Plugin dependencies
- `SdkVersion`: (Auto) SDK version
- `BuiltIn`: (Auto) Whether it's a built-in plugin
- `DllName`: (Auto) DLL file name
- `MainPlugin`: (Auto) Main plugin type
- `EntryPoints`: (Auto) Entry points

Metadata items (except `Dependencies`) are set in `plugin.json`.

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
        <PluginName>Emoji Plugin</PluginName>
        <Version>1.0.0.12</Version>
        <!-- PluginMeta --> <!-- [!code ++] -->
    </PropertyGroup>
</Project>
```

Create a new `plugin.json` in the root directory.

You can use `{{}}` to get variables from `.csproj`.

`Id` must correspond to `PackageId`.

```json [plugin.json]
{
  "Id": "{{ PackageId }}",
  "Name": "{{ PluginName }}",
  "Version": "{{ Version }}"
}
```

::: tip Note

In `<PluginMeta>`, you can use `MSBuild` property variables:
- [MSBuild Reserved and Well-Known Properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022)
- [Common MSBuild Project Properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022)

:::

The metadata item `Dependencies` is used to specify the plugin's `dependent plugins`, allowing the plugin to load after its `dependent plugins`.

Unlike regular metadata items, it needs to be set in the `<ItemGroup>` in `.csproj`.

It also serves as the project's `nuget` package.

- Must specify `Label="Dependencies"`
- Must specify `Need="(1.0,2.0)"` for required version internally

```xml [ShadowExample.Plugin.Emoji.csproj]
<ItemGroup Label="Dependencies">
    <PackageReference Include="ShadowExample.Plugin.Hello" Version="1.2.1.2"  Need="(1.0,2.0)"  />
    <PackageReference Include="ShadowExample.Plugin.World" Version="1.3.0.0"  Need="(1.0,2.0)"  />
</ItemGroup>
```

## Plugin Main Class

The plugin main class needs to inherit from the custom plugin base class in the plugin loader.

And must use the `[MainPlugin]` attribute to specify the plugin main class, which will automatically generate corresponding plugin metadata.

```csharp [LocalPlugin.cs]
// Example plugin main class
namespace ShadowViewer.Plugin.Local;

[MainPlugin]
public partial class LocalPlugin : PluginBase
{
}
```

::: tip
You can directly use `LocalPlugin.MetaData` to get the plugin's metadata information.
:::

## Subsequent Development

Next, you can choose to override the default methods in the plugin base class.

Or:
- [Use I18N Internationalization](/advance/i18n)
- [Custom WinUI Controls](/plugin/control)
- [Custom Resource Dictionaries](/plugin/resourcedictionary)
- [Custom Resource Files](/plugin/assets)
