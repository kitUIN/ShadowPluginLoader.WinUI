# 创建插件基类

插件基类决定了每个插件能做什么。把大家都会用到的方法放在这里，之后写插件时就不用重复实现了。

在 SDK 中新建 `PluginBase.cs`，把前面定义的 `ExampleMetaData` 传给 `AbstractPlugin`：

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

构造函数把插件信息、日志和事件服务交给基类。写具体插件时，还需要实现 `DisplayName`，给插件一个显示名称。

## 常用成员

| 成员 | 什么时候用 |
| --- | --- |
| `MetaData` / `Id` | 读取插件信息和标识 |
| `Loaded()` | 插件加载好后，做一些初始化工作 |
| `protected Enabled()` / `Disabled()` | 插件启用时开始工作，禁用时停止工作 |
| `IsEnabled` | 读取或修改启用状态，修改后会保存 |
| `protected ResourceDictionaries` | 添加插件要用的资源字典 |
| `PlanUpgrade` / `PlanRemove` | 表示插件的升级、删除状态 |

通常把初始化代码放在 `Loaded()` 就够了。如果要重写 `Init()`，记得调用 `base.Init()`，这样资源字典才能正常加载。`Init()` 执行得比较早，这时还不能使用派生类中等待注入的属性。

要真正安排升级或删除，请调用[加载器的方法](/zh/plugin/install)，只修改 `PlanUpgrade` 或 `PlanRemove` 不会执行这些操作。
