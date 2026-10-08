# Plugin Events

Use `ShadowPluginLoader.WinUI.Services.IPluginEventService`. First share the interface and concrete registration as shown in [Host Initialization](/init/customloaderclass), then subscribe:

```csharp
using DryIoc;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Services;

var events = DiFactory.Services.Resolve<IPluginEventService>();
events.PluginLoaded += OnPluginLoaded;

void OnPluginLoaded(object? sender, PluginEventArgs e)
{
    System.Diagnostics.Debug.WriteLine($"{e.PluginId}: {e.Status}");
}
```

Unsubscribe with `events.PluginLoaded -= OnPluginLoaded` when disposing the subscriber. Events are synchronous and do not automatically dispatch to the UI thread; use the control's `DispatcherQueue` for UI updates.

`PluginEventArgs` contains `PluginId` and `Status`. The table describes actual current source behavior:

| Event | Current trigger |
| --- | --- |
| `PluginLoaded` | After adding the instance and calling `Loaded()`, before enabling it |
| `PluginEnabled` | `IsEnabled` changes from false to true, after `Enabled()` |
| `PluginDisabled` | `IsEnabled` changes from true to false, after `Disabled()` |
| `PluginPlanUpgrade` | Setting instance `PlanUpgrade=true` |
| `PluginUpgraded` | Setting instance `PlanUpgrade=false` |
| `PluginPlanRemove` | Setting instance `PlanRemove=true` or calling `RemovePlugin`; current `UpgradePlugin` also raises this event |
| `PluginRemoved` | Exposed by the service, but not raised by the current default removal checker |

Plugins with persisted disabled state are still instantiated and raise `PluginLoaded`. Their initial false state does not raise `PluginDisabled`.

::: warning Current update/removal event limitations
`UpgradePlugin` currently calls `InvokePluginPlanRemove` with status `PlanRemove`, rather than the planned-upgrade event. Startup upgrade/removal checkers execute plans without raising `PluginUpgraded` / `PluginRemoved`. Do not use these events to determine operation completion; await `CheckUpgradeAndRemoveAsync()` and check subsequent loading results. Directly assigning `PlanUpgrade` / `PlanRemove` changes instance state and notifications without scheduling disk operations.
:::
