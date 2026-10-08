# Install, Update, and Remove Plugins

## Load at startup

After [initializing the host](/init/customloaderclass), run this on the UI thread:

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

`CheckUpgradeAndRemoveAsync()` performs pending removals before upgrades and sets the flags required by the processor. Await it at each startup before loading any plugins.

`ProcessAsync()` preprocesses inputs, reads metadata, sorts dependencies, loads assemblies, registers services, and instantiates plugins. No separate `Load()` call is needed. Public `Load(IEnumerable<string>, IProgress<PipelineProgress>?)` works on IDs already in the metadata cache and is normally called by pipeline `Outbound()`.

## Input sources

Import `ShadowPluginLoader.WinUI.Extensions` for these `Feed` overloads.

| Input | Current behavior |
| --- | --- |
| `DirectoryInfo` | Recursively finds `plugin.json`; does not automatically install `.sdow` files in that directory |
| `FileInfo` or local `Uri` | Reads `plugin.json` or extracts `.sdow` |
| HTTP / HTTPS `Uri` | Downloads and processes an archive, not standalone remote JSON |
| `Type` / `Feed<TPlugin>()` | Finds `{assembly name}/plugin.json` beside the assembly; DLL and metadata files are still required |
| `Type[]` / `IEnumerable<Type?>` | Adds metadata locations for each type |
| `Windows.ApplicationModel.Package` | Recursively scans the package installation directory |
| `IMaterial` | Uses the matching preprocessor |

Use `DirectoryInfo` for directories: the current URI branch does not correctly handle ordinary local directory URIs. Installed plugins default to `ApplicationData.Current.LocalFolder.Path/plugin`; downloads use `temp`. Configure these through `BaseSdkConfig`.

## Packages and progress

Continuing with the same `loader`, feed dependencies and their plugins together:

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

For network installation, use an HTTPS URI pointing to a plugin archive. Processing consumes and clears the current input list. Reinstalling an already loaded assembly does not hot-update it.

`PipelineProgress` provides `Step`, `SubStep`, status text, and percentage fields. Some current stages report inconsistent percentage scales, so do not assume every value is a normalized 0–1 fraction. Cancellation is passed to preprocessing and main processing; it does not guarantee rollback of extracted files or loaded assemblies.

Handle `ProcessAsync()` exceptions and inspect logs. Some metadata/extraction failures are logged and skipped; processing with no products does not report `Success`. Use `GetPlugin(id)` afterward to verify that required plugins loaded.

## Update and remove

These lines demonstrate separate update and removal actions; call them as appropriate:

```csharp
await loader.UpgradePlugin("ShadowExample.Plugin.Emoji",
    new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.2.0.sdow"));
await loader.RemovePlugin("ShadowExample.Plugin.Hello");
```

`UpgradePlugin(string id, Uri uri)` accepts only a local `.sdow` file URI. Download network packages first. The current plugin must already be loaded, the new version must be higher, and dependency versions must satisfy their constraints. Keep the update archive until the next startup finishes processing the plan.

`RemovePlugin(string id)` also requires a loaded plugin. Both methods persist plans executed by `CheckUpgradeAndRemoveAsync()` after restart; they do not unload assemblies in the running process. Do not use the installation pipeline to update an already loaded plugin.

## Query and change enabled state

```csharp
var plugins = loader.GetPlugins();
var enabledPlugins = loader.GetPlugins(isEnabled: true);
var plugin = loader.GetPlugin("ShadowExample.Plugin.Emoji");
bool? enabled = loader.IsEnabled("ShadowExample.Plugin.Emoji");
loader.DisablePlugin("ShadowExample.Plugin.Emoji");
loader.EnablePlugin("ShadowExample.Plugin.Emoji");
```

`GetPlugin` and `IsEnabled` return `null` for missing plugins. Disabling does not unload an instance or its resources. See [Plugin Events](/plugin/event) for actual event behavior.
