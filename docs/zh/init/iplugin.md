# 创建插件基类

先定义[元数据](/zh/init/metaplugin)，再将同一类型传入 `AbstractPlugin<TMeta>`。构造函数必须把元数据、日志和事件服务传给基类。

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

具体插件必须实现 `public override string DisplayName`。可在 SDK 基类中添加自己的业务方法。

| 成员 | 用途 |
| --- | --- |
| `MetaData` / `Id` | 当前插件实例的元数据与标识 |
| `Loaded()` | 实例加入加载器后调用，早于 `PluginLoaded` 事件 |
| `protected Enabled()` / `Disabled()` | 启用状态变化时调用，早于对应事件 |
| `IsEnabled` | 持久化启用状态，并在值变化时调用回调 |
| `protected ResourceDictionaries` | 初始化时合并到应用资源的字典路径 |
| `PlanUpgrade` / `PlanRemove` | 实例状态与事件；赋值本身不会创建磁盘操作计划 |

`Init()` 在基类构造函数内执行；覆写时要保留 `base.Init()` 才会合并资源字典。此时派生类的依赖注入属性尚未由构造函数赋值。升级和删除应调用[加载器接口](/zh/plugin/install)。
