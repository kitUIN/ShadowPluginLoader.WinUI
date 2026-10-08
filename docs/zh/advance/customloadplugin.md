# 自定义插件加载逻辑

## 实例化钩子

在 SDK 的加载器类中覆写以下钩子；这是替换前文空加载器的完整示例：

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

当前单个插件的调用顺序为：

1. `BeforeLoadPlugin(Type, TMeta)`。
2. `LoadMainPlugin(Type, TMeta)`：按 `meta.Id` 作为服务键解析 `TAPlugin`。
3. `AfterLoadPlugin(Type, TAPlugin, TMeta)`。
4. 将实例放入加载器字典，调用 `Loaded()`，触发 `PluginLoaded`。
5. 如果保存的状态为启用，设置 `IsEnabled`，调用 `Enabled()` 并触发 `PluginEnabled`。

`AfterLoadPlugin` 执行时实例尚未进入加载器字典，应直接使用传入的 `instance`。`BeforeLoadPlugin` 时程序集已加载、配置与主类已经注册；若要在加载程序集前验证包，应扩展预处理阶段。

通常只覆写前后钩子。覆写 `LoadMainPlugin` 时返回 `TAPlugin`；覆写 `LoadPlugin(TMeta)` 会接管完整生命周期，须自行保留状态、事件和依赖记录。

## 流水线扩展点

| 扩展点 | 使用方式 |
| --- | --- |
| `IMaterial` | 描述输入，提供 `TypeName`、`Path`、`Raw` |
| `IPreprocessingProcessor` | 实现 `PreprocessAsync(IMaterial, CancellationToken)`，返回 `IWorkpiece` |
| `ProcessorRegistry.PreprocessingProcessors` | 在处理开始前按 `TypeName` 注册预处理器 |
| `IMainProcessor` | 实现 `MainProcessAsync(...)`，完成工作件到 `IProduct` 的转换 |
| `IPluginFactory` | 提供 `CreatePipeline()` 和 `Outbound(...)` |

默认注册了本地 JSON、压缩包、HTTP 下载三种预处理器。未知 `TypeName` 会被流水线跳过；注册表是普通字典，应在启动阶段配置完毕。

替换主处理器时，在 `DiFactory.Init` 之后、解析加载器之前，用 DryIoc 重新注册 `IMainProcessor` 并指定 `IfAlreadyRegistered.Replace`。默认加载器的 `Outbound` 仍从依赖检查器的 `LoadedMetas` 取元数据；自定义主处理器须维护该缓存和主类 DI 注册，或同时提供自定义工厂。

流程图见[加载流程](/zh/detail/detail)。
