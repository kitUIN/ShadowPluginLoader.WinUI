# Plugin Events

## Description

Can be used through [`PluginEventService`](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/PluginEventService.cs) in dependency injection.

- [PluginLoaded Plugin Loaded Event](/plugin/event.html#插件加载完成事件)
- [PluginEnabled Plugin Enabled Event](/plugin/event.html#插件启用事件)
- [PluginDisabled Plugin Disabled Event](/plugin/event.html#插件关闭事件)
- [PluginPlanUpgrade Plugin Plan Upgrade Event](/plugin/event.html#插件计划升级事件)
- [PluginUpgraded Plugin Upgraded Event](/plugin/event.html#插件升级事件)
- [PluginPlanRemove Plugin Plan Remove Event](/plugin/event.html#插件计划删除事件)
- [PluginRemoved Plugin Removed Event](/plugin/event.html#插件删除事件)

## Plugin Loaded Event

Description:
- Triggered after plugin loading is complete
- Later than the `Loaded` function in the plugin class
- Earlier than the `Enabled` function in the plugin class
- Earlier than the `PluginEnabled` event

Trigger conditions:
- When plugin is loaded for the first time (even in disabled state)

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.Loaded`

## Plugin Enabled Event

Description:
- Triggered after plugin is enabled
- Later than the `Enabled` function in the plugin class

Trigger conditions:
- Plugin not loaded -> enabled
- Plugin disabled -> enabled

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.Enabled`

## Plugin Disabled Event

Description:
- Triggered after plugin is disabled
- Later than the `Disabled` function in the plugin class

Trigger conditions:
- ~~Plugin not loaded -> disabled~~
- Plugin enabled -> disabled

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.Disabled`

## Plugin Plan Upgrade Event

Description:
- Triggered when plugin upgrade is planned

Trigger conditions:
- Plugin upgrade planned

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.PlanUpgrade`

## Plugin Upgraded Event

Description:
- Triggered when plugin upgrade is complete

Trigger conditions:
- Plugin upgrade complete (usually after program restart)

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.Upgraded`

## Plugin Plan Remove Event

Description:
- Triggered when plugin removal is planned

Trigger conditions:
- Plugin removal planned

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.PlanRemove`

## Plugin Removed Event

Description:
- Triggered when plugin removal is complete

Trigger conditions:
- Plugin removal complete (usually after program restart)

Parameters: `PluginEventArgs`
- `PluginId` Plugin ID
- `Status` Status, fixed as `PluginStatus.Removed`
