# Create a Plugin Base Class

Define your [metadata](/init/metaplugin) first and pass the same type to `AbstractPlugin<TMeta>`. Forward metadata, logging, and the event service to the base constructor.

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

Concrete plugins must implement `public override string DisplayName`. Add application-specific methods to your SDK base class as needed.

| Member | Purpose |
| --- | --- |
| `MetaData` / `Id` | Metadata and identifier of this plugin instance |
| `Loaded()` | Called after adding the instance to the loader, before `PluginLoaded` |
| `protected Enabled()` / `Disabled()` | Called when enabled state changes, before the corresponding event |
| `IsEnabled` | Persists enabled state and invokes callbacks when the value changes |
| `protected ResourceDictionaries` | Dictionary paths merged into application resources during initialization |
| `PlanUpgrade` / `PlanRemove` | Instance state and events; assigning these does not schedule disk operations |

`Init()` runs inside the base constructor. Keep `base.Init()` when overriding it to retain resource merging. Derived dependency properties have not yet been assigned by the constructor at this point. Schedule upgrades and removals through the [loader APIs](/plugin/install).
