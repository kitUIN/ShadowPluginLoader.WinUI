# Custom Plugin Loading Logic

## Instantiation hooks

Override the following hooks in your SDK loader. This complete example replaces the empty loader from the quick start:

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

The current per-plugin sequence is:

1. `BeforeLoadPlugin(Type, TMeta)`.
2. `LoadMainPlugin(Type, TMeta)`, resolving `TAPlugin` with service key `meta.Id`.
3. `AfterLoadPlugin(Type, TAPlugin, TMeta)`.
4. Add the instance to the loader, call `Loaded()`, and raise `PluginLoaded`.
5. If the persisted state is enabled, set `IsEnabled`, call `Enabled()`, and raise `PluginEnabled`.

The instance is not in the loader dictionary during `AfterLoadPlugin`; use its argument directly. Assemblies have already loaded and configurations/main classes have been registered by `BeforeLoadPlugin`. Validate packages in preprocessing if validation must precede assembly loading.

Usually the before/after hooks are sufficient. An override of `LoadMainPlugin` must return `TAPlugin`. Overriding `LoadPlugin(TMeta)` replaces the entire lifecycle, including state, events, and dependency records.

## Pipeline extension points

| Extension point | Usage |
| --- | --- |
| `IMaterial` | Describes input with `TypeName`, `Path`, and `Raw` |
| `IPreprocessingProcessor` | Implements `PreprocessAsync(IMaterial, CancellationToken)`, returning `IWorkpiece` |
| `ProcessorRegistry.PreprocessingProcessors` | Registers preprocessors by `TypeName` before processing starts |
| `IMainProcessor` | Implements `MainProcessAsync(...)` to convert workpieces to `IProduct` results |
| `IPluginFactory` | Provides `CreatePipeline()` and `Outbound(...)` |

The defaults handle local JSON, archives, and HTTP downloads. Unknown material types are skipped. The registry is an ordinary dictionary; configure it during startup.

To replace the main processor, register `IMainProcessor` in DryIoc with `IfAlreadyRegistered.Replace` after `DiFactory.Init` and before resolving the loader. The default loader's `Outbound` still reads metadata from the dependency checker's `LoadedMetas`. A custom processor must maintain this cache and main-class DI registrations, or be paired with a custom factory.

See the [loading flow](/detail/detail).
