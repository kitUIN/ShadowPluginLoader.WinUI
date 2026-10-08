# Plugin Events

Want to refresh a list when a plugin loads or show a message when it's enabled? Subscribe to events from `IPluginEventService`.

After [initializing your app](/init/customloaderclass), try listening for loaded plugins:

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

`e.PluginId` identifies the plugin, and `e.Status` tells you what happened. Use `events.PluginLoaded -= OnPluginLoaded` when you no longer need the subscription. For UI changes, use the control's `DispatcherQueue` to return to the UI thread.

## Loading and enabled-state events

| Event | When it fires |
| --- | --- |
| `PluginLoaded` | After the plugin loads and finishes `Loaded()` |
| `PluginEnabled` | After changing from disabled to enabled and finishing `Enabled()` |
| `PluginDisabled` | After changing from enabled to disabled and finishing `Disabled()` |

Loading doesn't necessarily enable a plugin. A plugin that was disabled during the previous run still loads at startup but stays disabled.

## Update and removal events

Pay attention to the conditions for these events:

| Event | When it fires |
| --- | --- |
| `PluginPlanUpgrade` | When you set the plugin's `PlanUpgrade=true` |
| `PluginUpgraded` | When you set `PlanUpgrade=false` |
| `PluginPlanRemove` | When you set `PlanRemove=true` or call `RemovePlugin`; `UpgradePlugin` currently raises it too |
| `PluginRemoved` | Not yet raised by the default removal workflow |

To confirm an update or removal, wait for `CheckUpgradeAndRemoveAsync()` at the next startup and check the plugin afterward. Don't rely on these events alone for the result.

Use the [update and removal methods](/plugin/install) to schedule operations. Assigning `PlanUpgrade` or `PlanRemove` only changes state and sends notifications.
