# Customize Plugin Packaging

The defaults work for most plugins. If you want another output folder, a different filename, or your own build workflow, here's where to change them.

## Change the output folder and filename

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

`PluginPackagePath` sets the output folder, `PluginPackageName` sets the filename, and `PluginPackageExt` sets the extension. Change `PluginPackagePath`, for example, and the next build will put its package in that folder.

Debug builds also add `-Debug` to the name. Keep `.sdow` as the extension to use the loader's standard install and update methods.

This file replaces the tool's default configuration. Start with the complete example above and adjust what you need.

## Adjust exclusions

List unwanted files in your project's `Plugin.Build.exclude`; see [Plugin Packaging](/plugin/pack) for the format. When that file isn't present, the tool uses its own default list.

Include any shared dependencies you still want excluded. Your list and the default list aren't merged automatically.

## Use your own build workflow

For further customization, replace the relevant MSBuild files:

| Property / local file | Steps it handles |
| --- | --- |
| `ToolTargetsFile` / `Tool.targets` | General tasks such as copying `Tools.Config.props` |
| `LoaderToolTargetsFile` / `Loader.Build.targets` | Exporting SDK metadata and creating NuGet packages |
| `PluginToolTargetsFile` / `Plugin.Build.targets` | Copying definitions, generating plugin metadata, and packaging |
| `LoaderPropsFile` / `Loaders.Build.props` | Telling plugin projects where to find `PluginDFile` |
| `ExcludeFile` | Selecting exclusion lists; separate multiple paths with semicolons |

Copy the tool's default file first, then change the steps you need. Keep metadata generation and resource copying so the package has everything required for loading.

## Create an MSIX package

Set `PluginMisxPackage` to `true` and supply the certificate path in `AppCertPath` and password in `AppCertPassword`. The build then uses `PackageMsix.ps1` to create an additional package.

Use the installation pipeline for `.sdow` files. After installing an MSIX package in Windows, use `Feed(Package)` to find its plugins.
