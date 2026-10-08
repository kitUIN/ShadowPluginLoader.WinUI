# Plugin Packaging

Set `IsPlugin=true`, `IsPluginLoader=false`, and `AutoPluginPackage=true` in `Tools.Config.props`, then build the plugin.

## Output

Packages default to `$(ProjectDir)Packages/`:

- Release: `$(TargetName)-$(Version).sdow`.
- Debug: `$(TargetName)-$(Version)-Debug.sdow`.

`.sdow` uses ZIP format and is created from the plugin build output. It contains assemblies, generated metadata, XAML, and resources. This abbreviated layout shows the important relationship; retain generated PRI files and required dependencies as well:

```text
ShadowExample.Plugin.Emoji.dll
ShadowExample.Plugin.Emoji/
  plugin.json
  Themes/
    ResourceDictionary1.xaml
  Assets/
    th.jpg
```

`plugin.json` is inside the assembly-named subdirectory, with the DLL one level above it. Do not package only the source template or DLL. The archive preprocessor searches for ZIP entries ending in `/plugin.json`.

## Exclude files

Create `Plugin.Build.exclude` in the project directory, with one pattern per line:

```text [Plugin.Build.exclude]
*.pdb
hello.*
Fluent
```

The current packager recursively enumerates output and matches **file or directory names**, case-insensitively. `*` matches any characters and `?` matches one character. `Fluent` matches that directory name at any depth. Path patterns such as `Fluent/*` or `core/**/text.txt` do not match because comparisons do not use relative paths.

The project list replaces the tool's default list. Exclusion actually deletes matching files/directories from build output before creating the archive; source files are unaffected. Rebuild before reusing the output for another deployment workflow.

See [Custom Packaging](/advance/custompluginbuild) for naming, target replacement, and optional MSIX support, or [Installation and Management](/plugin/install) to load the archive.
