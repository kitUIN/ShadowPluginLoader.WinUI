# Tools.Config.props

This file tells the build tool whether a project is an SDK or a plugin, whether to package it automatically, and whether to generate localization helpers.

The tool copies a default file into your project on the first build. You can also create it yourself:

```xml [Tools.Config.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <IsPluginLoader>false</IsPluginLoader>
    <IsPlugin>false</IsPlugin>
    <AutoPluginPackage>true</AutoPluginPackage>
    <PluginMisxPackage>false</PluginMisxPackage>
    <AutoGenerateI18N>true</AutoGenerateI18N>
    <DebugSourceGenerator>false</DebugSourceGenerator>
  </PropertyGroup>
</Project>
```

## Choose the project type

- SDK: `IsPluginLoader=true`, `IsPlugin=false`.
- Plugin: `IsPluginLoader=false`, `IsPlugin=true`.
- Ordinary app: leave both `false`.

Enable only one of `IsPluginLoader` and `IsPlugin`.

## Other switches

| Property | Default | What it does |
| --- | --- | --- |
| `AutoPluginPackage` | `true` | Creates an installation package when you build a plugin |
| `PluginMisxPackage` | `false` | Also creates an MSIX package; requires a certificate |
| `AutoGenerateI18N` | `true` | Generates localization helpers from resources in `Strings` |
| `DebugSourceGenerator` | `false` | Saves generated plugin source in `GeneratedFiles` for inspection |

You still write the `plugin.json` template yourself. The build tool fills in its variables and creates the final file. If you turn off automatic packaging, include metadata generation in your own build workflow; see [Custom Packaging](/advance/custompluginbuild).

Turn on `DebugSourceGenerator` to see the generated code. Treat those files as a reference: make changes in your own source files, then rebuild.

See [Internationalization](/advance/i18n) and [Quick Dependency Injection](/advance/quickdi) for localization and generated constructors.
