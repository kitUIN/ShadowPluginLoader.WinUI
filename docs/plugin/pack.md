# Plugin Packaging

Once your plugin is ready, package it as a `.sdow` file for the app to install.

Check `Tools.Config.props`: set `IsPlugin` and `AutoPluginPackage` to `true`, and `IsPluginLoader` to `false`. Then build the project as usual.

## Find the package

After building, open the project's `Packages` folder. The default filenames are:

- Release: `$(TargetName)-$(Version).sdow`.
- Debug: `$(TargetName)-$(Version)-Debug.sdow`.

For example, an Emoji debug package could be named `ShadowExample.Plugin.Emoji-1.1.0-Debug.sdow`.

A `.sdow` file is a ZIP archive containing the build output: DLLs, plugin metadata, and resources. The main layout looks like this, alongside generated PRI files and dependencies:

```text
ShadowExample.Plugin.Emoji.dll
ShadowExample.Plugin.Emoji/
  plugin.json
  Themes/
    ResourceDictionary1.xaml
  Assets/
    th.jpg
```

Keep `plugin.json` in the assembly-named subfolder, with the DLL one level above it. Using the generated package saves you from arranging these files yourself.

## Leave out files you don't need

Create `Plugin.Build.exclude` in the project directory. Put one name or wildcard pattern on each line:

```text [Plugin.Build.exclude]
*.pdb
hello.*
Fluent
```

This excludes `.pdb` files, files matching `hello.*`, and directories named `Fluent`.

`*` matches any characters, and `?` matches one character. Matching ignores case and uses only the file or directory name, so write `Fluent`, not `Fluent/*`.

Your list replaces the default list, so include any shared dependencies you still want excluded. Packaging deletes matches from the **build output directory** before creating the archive. Rebuild if you need to use that output for another deployment.

See [Custom Packaging](/advance/custompluginbuild) to change the output location or filename. Once you have a package, [install and try it](/plugin/install).
