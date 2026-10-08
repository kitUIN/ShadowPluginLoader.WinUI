# 插件事件

事件接口为 `ShadowPluginLoader.WinUI.Services.IPluginEventService`。先完成[宿主初始化](/zh/init/customloaderclass)中的接口/具体类型共享注册，再订阅：

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

订阅者销毁时使用 `events.PluginLoaded -= OnPluginLoaded` 解除订阅。事件同步触发，不保证自动切换到 UI 线程；更新控件时使用其 `DispatcherQueue`。

`PluginEventArgs` 提供 `PluginId` 和 `Status`。下表按当前源码的实际触发位置说明：

| 事件 | 当前触发条件 |
| --- | --- |
| `PluginLoaded` | 实例加入加载器并执行 `Loaded()` 后；早于启用回调 |
| `PluginEnabled` | `IsEnabled` 从 false 变为 true，在 `Enabled()` 之后 |
| `PluginDisabled` | `IsEnabled` 从 true 变为 false，在 `Disabled()` 之后 |
| `PluginPlanUpgrade` | 对插件实例设置 `PlanUpgrade=true` |
| `PluginUpgraded` | 对插件实例设置 `PlanUpgrade=false` |
| `PluginPlanRemove` | 设置实例 `PlanRemove=true`，或调用 `RemovePlugin`；当前 `UpgradePlugin` 也触发此事件 |
| `PluginRemoved` | 服务公开此事件，但当前默认删除检查器没有触发它 |

插件保存为禁用状态时仍会实例化并触发 `PluginLoaded`；初始 false 不会触发 `PluginDisabled`。

::: warning 当前更新/删除事件的限制
`UpgradePlugin` 当前调用的是 `InvokePluginPlanRemove`，状态也为 `PlanRemove`，不是计划升级事件。启动时的升级/删除检查器只执行计划，不发送 `PluginUpgraded` / `PluginRemoved`。因此不能依靠这些事件判断更新或删除完成；应等待 `CheckUpgradeAndRemoveAsync()` 并核对后续加载结果。直接设置 `PlanUpgrade` / `PlanRemove` 仅改变实例状态和通知，不会创建操作计划。
:::
