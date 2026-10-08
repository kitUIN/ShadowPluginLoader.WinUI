# 插件事件

想在插件加载好后更新列表，或者在用户启用插件时显示提示？可以订阅 `IPluginEventService` 提供的事件。

完成[主程序初始化](/zh/init/customloaderclass)后，先试着监听加载完成事件：

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

`e.PluginId` 告诉你是哪个插件，`e.Status` 告诉你发生了什么。不再监听时，用 `events.PluginLoaded -= OnPluginLoaded` 取消订阅。需要更新界面的话，记得通过控件的 `DispatcherQueue` 回到 UI 线程。

## 加载和启用事件

| 事件 | 什么时候触发 |
| --- | --- |
| `PluginLoaded` | 插件加载好，并执行完 `Loaded()` 之后 |
| `PluginEnabled` | 从禁用变为启用，并执行完 `Enabled()` 之后 |
| `PluginDisabled` | 从启用变为禁用，并执行完 `Disabled()` 之后 |

加载完成不代表已经启用。插件如果上次是禁用状态，启动时也会加载，但不会自动启用。

## 更新和删除相关事件

这部分事件使用时要留意触发条件：

| 事件 | 什么时候触发 |
| --- | --- |
| `PluginPlanUpgrade` | 设置插件的 `PlanUpgrade=true` 时 |
| `PluginUpgraded` | 设置插件的 `PlanUpgrade=false` 时 |
| `PluginPlanRemove` | 设置 `PlanRemove=true`，或调用 `RemovePlugin` 时；`UpgradePlugin` 目前也会触发它 |
| `PluginRemoved` | 默认删除流程暂时不会触发 |

要确认更新或删除是否完成，请等待下次启动的 `CheckUpgradeAndRemoveAsync()` 执行完，再检查插件。不要仅凭上面的事件判断结果。

安排操作时仍然使用[更新和删除方法](/zh/plugin/install)，直接修改 `PlanUpgrade`、`PlanRemove` 只会改变状态和发送通知。
