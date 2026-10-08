# Customize Plugin Packaging

## Output directory and name

Create `Plugin.Build.props` in the plugin project directory:

```xml [Plugin.Build.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PluginPackagePath>$(ProjectDir)Packages/</PluginPackagePath>
    <PluginPackageName>$(TargetName)-$(Version)</PluginPackageName>
    <PluginPackageExt>.sdow</PluginPackageExt>
  </PropertyGroup>
</Project>
```

This replaces the tool package's default file, so retain all defaults you need. `PluginPackagePath` sets the directory, `PluginPackageName` sets the name without its extension, and `PluginPackageExt` sets the extension. Debug builds append `-Debug`.

Keep `.sdow` for compatibility with default `Feed(Uri)` and upgrade APIs. Changing the extension does not change the archive format. Other extensions need custom input handling; the upgrade API still accepts only `.sdow`.

## Exclusions

The project-level `Plugin.Build.exclude` is used when present; otherwise the tool's built-in list is used. Your list replaces the default, so retain exclusions for shared host assemblies as needed. See [Plugin Packaging](/plugin/pack).

## Replace MSBuild targets

| Property / local file | Default responsibility |
| --- | --- |
| `ToolTargetsFile` / `Tool.targets` | General targets, including copying `Tools.Config.props` |
| `LoaderToolTargetsFile` / `Loader.Build.targets` | SDK metadata export and NuGet packaging |
| `PluginToolTargetsFile` / `Plugin.Build.targets` | Plugin schema copying, template generation, and packaging |
| `LoaderPropsFile` / `Loaders.Build.props` | Passes `PluginDFile` from the SDK package to plugins |
| `ExcludeFile` | Exclusion list path; the tool supports semicolon-separated files |

Replacing a target file takes over its entire workflow. Preserve required metadata generation and resource copying. Although default props declare `DefaultExclude`, current packaging code does not use it to toggle built-in rules; control exclusions through the actual lists.

## Optional MSIX

The tool's switch is spelled `PluginMisxPackage`, with certificate settings `AppCertPath` and `AppCertPassword`. It invokes `PackageMsix.ps1` after ordinary packaging and requires a certificate and packaging tools. This does not make `.msix` archives valid default pipeline inputs; `Feed(Package)` accepts an already installed Windows package.
