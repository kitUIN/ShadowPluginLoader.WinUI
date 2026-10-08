# Tools.Config.props

On the first build, the tool copies default `Tools.Config.props` into the project directory. You can also create it beforehand and configure the project role. The filename uses plural `Tools`.

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

| Property | Default | Behavior |
| --- | --- | --- |
| `IsPluginLoader` | `false` | Exports the `[ExportMeta]` schema and packages it in the SDK NuGet package |
| `IsPlugin` | `false` | Imports plugin targets, copies `plugin.d.json`, generates metadata, and packages output |
| `AutoPluginPackage` | `true` | Runs plugin packaging during build |
| `PluginMisxPackage` | `false` | Also invokes the MSIX script; requires certificate configuration and this exact spelling |
| `AutoGenerateI18N` | `true` | Supplies `Strings/**/*` as generator additional files |
| `DebugSourceGenerator` | `false` | Writes generated source to `GeneratedFiles` for plugin projects |

SDKs set `IsPluginLoader=true` and `IsPlugin=false`; plugins do the reverse. Ordinary hosts leave both false. Do not enable both; current target imports prioritize the SDK branch in that case.

Write the source `plugin.json` yourself. The tool renders and validates it, then generates the output file. `ReadMetaData` currently runs before `PackagePlugin`; when disabling automatic packaging, explicitly retain metadata generation in your custom workflow and verify the output JSON is up to date.

Generated files are for inspection; do not edit them or add them to compilation twice. Constructor generation is triggered by `[Autowired]` / `[CheckAutowired]`, independently of `AutoGenerateI18N`.

See [Custom Packaging](/advance/custompluginbuild), [Dependency Injection](/advance/quickdi), and [Internationalization](/advance/i18n).
