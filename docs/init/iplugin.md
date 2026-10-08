# Create a Plugin Base Class

The base class defines what your plugins can do. Put shared behavior here so each plugin doesn't have to implement it again.

Add `PluginBase.cs` to the SDK and pass your `ExampleMetaData` type to `AbstractPlugin`:

```csharp [PluginBase.cs]
using Serilog;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase : AbstractPlugin<ExampleMetaData>
{
    protected PluginBase(ExampleMetaData meta, ILogger logger,
        PluginEventService pluginEventService)
        : base(meta, logger, pluginEventService)
    {
    }
}
```

The constructor passes plugin metadata, logging, and events to the base class. Each concrete plugin also needs a `DisplayName` property for its display name.

## Members you'll use

| Member | When to use it |
| --- | --- |
| `MetaData` / `Id` | Read the plugin's information and identifier |
| `Loaded()` | Set things up once the plugin has loaded |
| `protected Enabled()` / `Disabled()` | Start work when enabled and stop it when disabled |
| `IsEnabled` | Read or change the enabled state; changes are saved |
| `protected ResourceDictionaries` | Add resource dictionaries for the plugin |
| `PlanUpgrade` / `PlanRemove` | Represent upgrade and removal state |

`Loaded()` is usually a good place for initialization. If you override `Init()`, call `base.Init()` so resource dictionaries still load. It runs early, before injected properties in your derived class are assigned.

To schedule an upgrade or removal, use the [loader methods](/plugin/install). Changing `PlanUpgrade` or `PlanRemove` alone won't schedule the operation.
