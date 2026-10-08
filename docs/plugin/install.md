# Install, Update, and Remove Plugins

Now that you have a loader and a plugin, let's load the plugin in your app.

## Load plugins at startup

After [initializing your app](/init/customloaderclass), call this on the window's UI thread:

```csharp
using DryIoc;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Extensions;
using System.IO;

var loader = DiFactory.Services.Resolve<ShadowExamplePluginLoader>();
await loader.CheckUpgradeAndRemoveAsync();
await loader.CreatePipeline()
    .Feed(new DirectoryInfo(loader.PluginFolderPath))
    .ProcessAsync();
```

Start with `CheckUpgradeAndRemoveAsync()` to handle removals and updates scheduled during the previous run. Wait for it to finish before loading plugins.

The next three calls set up and run a loading task: `CreatePipeline()` creates it, `Feed(...)` tells it where to look, and `ProcessAsync()` starts the work. The loader reads metadata, checks dependencies, and creates plugin instances. You don't need a separate `Load()` call.

## Where can plugins come from?

Add `using ShadowPluginLoader.WinUI.Extensions;` to use these `Feed` overloads:

| Argument | How to use it |
| --- | --- |
| `DirectoryInfo` | Find `plugin.json` in a directory and its subdirectories |
| `FileInfo` or local `Uri` | Point to a `plugin.json` file or `.sdow` package |
| HTTP / HTTPS `Uri` | Download and install a plugin archive |
| `Type` / `Feed<TPlugin>()` | Load the plugin for that type; requires `{assembly name}/plugin.json` beside the assembly |
| `Type[]` / `IEnumerable<Type?>` | Supply several plugin types together |
| `Windows.ApplicationModel.Package` | Find plugins in an installed Windows package |
| `IMaterial` | Use [custom processing](/advance/customloadplugin) |

Use `DirectoryInfo` when scanning a folder. It finds unpacked plugins. For `.sdow` files, pass each package path to `Feed` instead.

Plugins are installed in the `plugin` folder under the app's local data directory. Downloads go into `temp`. Change these locations through `BaseSdkConfig` if needed.

## Install a package

Using the same `loader`, pass in your package paths. This example installs Hello and Emoji together and reports progress:

```csharp
using System;
using System.Diagnostics;
using System.Threading;
using ShadowPluginLoader.WinUI.Models;

var progress = new Progress<PipelineProgress>(p =>
    Debug.WriteLine($"{p.Step}: {p.SubStep} {p.TotalStatusValue}"));
using var cancellation = new CancellationTokenSource();

await loader.CreatePipeline()
    .Feed(new Uri(@"C:\Plugins\ShadowExample.Plugin.Hello-1.1.6.sdow"))
    .Feed(new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.1.0.sdow"))
    .ProcessAsync(progress, cancellation.Token);
```

To install from the web, use the archive's HTTPS address. Call `cancellation.Cancel()` to request cancellation; files already extracted and assemblies already loaded won't be rolled back.

The example reports `Step`, `SubStep`, and status text. Percentage calculations aren't yet consistent across stages, so use the steps to show progress for now.

Catch exceptions and check the logs if installation fails. After processing, call `GetPlugin(id)` to see whether the plugin loaded successfully.

## Update and remove

Updates and removals take effect after restarting the app. These lines show the two operations separately:

```csharp
await loader.UpgradePlugin("ShadowExample.Plugin.Emoji",
    new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.2.0.sdow"));
await loader.RemovePlugin("ShadowExample.Plugin.Hello");
```

For updates, provide a local `.sdow` path. Download web packages first. The plugin must already be loaded, the new version must be higher, and its dependencies must be satisfied. Keep the update archive until the next startup has used it.

Removal also applies to loaded plugins. On the next launch, `CheckUpgradeAndRemoveAsync()` carries out these operations.

## Read and change enabled state

```csharp
var plugins = loader.GetPlugins();
var enabledPlugins = loader.GetPlugins(isEnabled: true);
var plugin = loader.GetPlugin("ShadowExample.Plugin.Emoji");
bool? enabled = loader.IsEnabled("ShadowExample.Plugin.Emoji");
loader.DisablePlugin("ShadowExample.Plugin.Emoji");
loader.EnablePlugin("ShadowExample.Plugin.Emoji");
```

`GetPlugin` and `IsEnabled` return `null` when a plugin isn't found. Disabled plugins stay in memory, so each plugin should stop its work in `Disabled()`.

To show loading or enabled-state changes in your UI, see [Plugin Events](/plugin/event).
