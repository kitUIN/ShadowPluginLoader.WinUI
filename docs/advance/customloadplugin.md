# Custom Plugin Loading Logic

You may want to register extra services before a plugin starts or run some setup after it's created. Override the matching methods in your loader.

## Add work before and after creation

Update the SDK loader like this to log a message before and after each plugin is created:

```csharp [ShadowExamplePluginLoader.cs]
using System;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core;

[CheckAutowired]
public partial class ShadowExamplePluginLoader
    : AbstractPluginLoader<ExampleMetaData, PluginBase>
{
    protected override void BeforeLoadPlugin(Type plugin, ExampleMetaData meta)
    {
        base.BeforeLoadPlugin(plugin, meta);
        Logger.Information("Preparing {Id} ({Type})", meta.Id, plugin.FullName);
    }

    protected override void AfterLoadPlugin(Type plugin, PluginBase instance,
        ExampleMetaData meta)
    {
        base.AfterLoadPlugin(plugin, instance, meta);
        Logger.Information("Created {Name}", instance.DisplayName);
    }
}
```

These methods run in this order:

1. `BeforeLoadPlugin`: prepare to create the plugin; register any services it needs here.
2. `LoadMainPlugin`: get the plugin instance from DI.
3. `AfterLoadPlugin`: the instance is ready; use the supplied `instance` argument.
4. The loader stores the plugin, calls `Loaded()`, and raises `PluginLoaded`.
5. If the plugin was enabled, it calls `Enabled()` and raises `PluginEnabled`.

Use the `instance` argument inside `AfterLoadPlugin`. It isn't available through `GetPlugin` yet.

Overriding the before/after methods is usually enough. If you replace the whole `LoadPlugin(TMeta)` method, you also take responsibility for state, events, and dependency records.

## Check packages before loading a DLL

By the time `BeforeLoadPlugin` runs, the DLL is already loaded. Add a preprocessor if you need to check package contents earlier.

Each step in the pipeline has its own interface:

| Interface or class | What it handles |
| --- | --- |
| `IMaterial` | Describes an input, including its type name and path |
| `IPreprocessingProcessor` | Reads or downloads the input and returns an `IWorkpiece` |
| `ProcessorRegistry.PreprocessingProcessors` | Maps input type names to preprocessors |
| `IMainProcessor` | Checks metadata, loads assemblies, and returns `IProduct` results |
| `IPluginFactory` | Creates pipelines and finishes loading through `Outbound(...)` |

Local JSON, archives, and HTTP downloads are already supported. For another source, implement `IMaterial` and `IPreprocessingProcessor`, then register the processor by `TypeName` during startup.

To replace `IMainProcessor`, register it after `DiFactory.Init` and before resolving the loader, using `IfAlreadyRegistered.Replace`. If you keep the default loader, also maintain the `LoadedMetas` cache and main-class DI registrations: the loader needs them to create plugins.

See the [loading flow diagram](/detail/detail) to find where each step fits.
