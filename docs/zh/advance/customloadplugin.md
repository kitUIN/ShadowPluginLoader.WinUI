# 自定义插件加载逻辑

有时你希望在插件开始工作前注册几个服务，或者在创建后做一些额外初始化。可以在加载器中重写对应的方法。

## 在加载前后做点事情

把 SDK 中的加载器改成下面这样，就能在插件创建前后各写一条日志：

```csharp [ShadowExamplePluginLoader.cs]
using System;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core;

[CheckAutowired]
public partial class ShadowExamplePluginLoader
    : AbstractPluginLoader<ExampleMetaData, PluginBase>
{
    protected override void BeforeLoadPlugin(Type plugin, ExampleMetaData meta)
    {
        base.BeforeLoadPlugin(plugin, meta);
        Logger.Information("Preparing {Id} ({Type})", meta.Id, plugin.FullName);
    }

    protected override void AfterLoadPlugin(Type plugin, PluginBase instance,
        ExampleMetaData meta)
    {
        base.AfterLoadPlugin(plugin, instance, meta);
        Logger.Information("Created {Name}", instance.DisplayName);
    }
}
```

这几个方法会按下面的顺序调用：

1. `BeforeLoadPlugin`：准备创建插件，可以在这里注册它需要的服务。
2. `LoadMainPlugin`：从 DI 容器中取出插件实例。
3. `AfterLoadPlugin`：实例已经创建好，可以直接使用传入的 `instance`。
4. 加载器保存插件，调用 `Loaded()`，然后发出 `PluginLoaded` 事件。
5. 如果插件之前是启用状态，再调用 `Enabled()` 并发出 `PluginEnabled` 事件。

在 `AfterLoadPlugin` 中用参数里的 `instance` 就好，这时还不能通过 `GetPlugin` 找到它。

一般重写前后两个方法就够了。如果重写整个 `LoadPlugin(TMeta)`，你也需要负责保存状态、发送事件和记录依赖。

## 想在加载 DLL 前检查插件包

`BeforeLoadPlugin` 调用时，DLL 已经加载了。如果想更早检查包内容，可以添加自己的预处理器。

流水线分成几步，每一步都有对应接口：

| 接口或类 | 负责什么 |
| --- | --- |
| `IMaterial` | 描述要处理的输入，包括类型名称和路径 |
| `IPreprocessingProcessor` | 读取或下载输入，返回 `IWorkpiece` |
| `ProcessorRegistry.PreprocessingProcessors` | 保存输入类型名称与预处理器的对应关系 |
| `IMainProcessor` | 检查插件信息、加载程序集，返回 `IProduct` |
| `IPluginFactory` | 创建流水线，并在 `Outbound(...)` 中完成插件加载 |

默认已经支持本地 JSON、压缩包和 HTTP 下载。要增加一种来源，就实现 `IMaterial` 和 `IPreprocessingProcessor`，再在启动时把处理器按 `TypeName` 加入注册表。

如果还要替换 `IMainProcessor`，在 `DiFactory.Init` 之后、取出加载器之前重新注册，并使用 `IfAlreadyRegistered.Replace`。继续使用默认加载器时，要保留元数据缓存 `LoadedMetas` 和主类的 DI 注册，因为后续创建插件还会用到它们。

可以结合[加载流程图](/zh/detail/detail)看每一步的位置。
