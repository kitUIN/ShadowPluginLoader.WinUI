# 插件事件

## 说明

可以通过依赖注入中的[`PluginEventService`](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/PluginEventService.cs)进行使用

- [PluginLoaded 插件加载完成事件](/zh/plugin/event.html#插件加载完成事件)
- [PluginEnabled 插件启用事件](/zh/plugin/event.html#插件启用事件)
- [PluginDisabled 插件关闭事件](/zh/plugin/event.html#插件关闭事件)
- [PluginPlanUpgrade 插件计划升级事件](/zh/plugin/event.html#插件计划升级事件)
- [PluginUpgraded 插件升级事件](/zh/plugin/event.html#插件升级事件)
- [PluginPlanRemove 插件计划删除事件](/zh/plugin/event.html#插件计划删除事件)
- [PluginRemoved 插件删除事件](/zh/plugin/event.html#插件删除事件)

## 插件加载完成事件

描述: 
- 在插件加载完毕后触发
- 晚于插件类中的`Loaded`函数
- 早于插件类中的`Enabled`函数
- 早于`PluginEnabled`事件

触发条件:
- 插件第一次加载时(关闭状态也会加载)

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.Loaded`

## 插件启用事件

描述: 
- 在插件启用后触发
- 晚于插件类中的`Enabled`函数
  
触发条件:
- 插件未加载->启用时
- 插件关闭->启用时

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.Enabled`

## 插件关闭事件

描述: 
- 在插件关闭后触发
- 晚于插件类中的`Disabled`函数
  
触发条件:
- ~~插件未加载->关闭时~~
- 插件启用->关闭时

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.Disabled`

## 插件计划升级事件

描述: 
- 在插件计划升级时触发
  
触发条件:
- 插件计划升级

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.PlanUpgrade`

## 插件升级事件

描述: 
- 在插件升级完成时触发
  
触发条件:
- 插件升级完成(一般在程序重启后)

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.Upgraded`

## 插件计划删除事件

描述: 
- 在插件计划删除时触发
  
触发条件:
- 插件计划删除

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.PlanRemove`

## 插件删除事件

描述: 
- 在插件删除完成时触发
  
触发条件:
- 插件删除完成(一般在程序重启后)

参数: `PluginEventArgs`
- `PluginId` 插件Id
- `Status` 状态, 此处固定为`PluginStatus.Removed`