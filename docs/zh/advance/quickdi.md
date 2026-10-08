# 快速依赖注入

`ShadowPluginLoader.SourceGenerator` 生成构造函数，DryIoc 负责实际解析依赖。相关类需要声明为 `public partial`，特性来自 `ShadowPluginLoader.Attributes`。

## Autowired 属性

```csharp
using Serilog;
using ShadowPluginLoader.Attributes;

namespace ShadowExample.ViewModels;

public partial class StatusViewModel
{
    [Autowired]
    public ILogger Logger { get; }

    partial void ConstructorInit()
    {
        Logger.Information("StatusViewModel initialized");
    }
}
```

上例生成的构造函数等效于：

```csharp
public StatusViewModel(ILogger logger)
{
    Logger = logger;
    ConstructorInit();
}
```

生成器还声明 `partial void ConstructorInit()`，可在原类中实现它，执行依赖赋值后的初始化。不要再手写相同签名的构造函数。

## CheckAutowired 类

在派生类上标记 `[CheckAutowired]`，即使没有新增 `[Autowired]` 属性，也会检测并转发基类构造参数。例如[插件主类](/zh/plugin/create)会生成：

```csharp
public EmojiPlugin(ExampleMetaData meta, ILogger logger,
    PluginEventService pluginEventService)
    : base(meta, logger, pluginEventService)
{
    ConstructorInit();
}
```

这些片段用于解释生成结果，不要复制到已有生成构造函数的类中。空类上仅加 `[CheckAutowired]` 不会凭空增加服务参数；没有待注入参数时不会生成构造函数。

## 服务注册

特性不会自动注册业务服务。宿主须在解析之前用 `DiFactory.Services.Register<TService, TImplementation>()` 等方式注册服务。默认主处理器会按插件 ID 注册插件主类，并向它传入元数据；插件无需单独注册自己的主类。公开的配置类会按[配置章节](/zh/plugin/config)的规则载入并注册。

若控件使用生成构造函数，可在 `ConstructorInit()` 中调用 `this.LoadComponent(ref _contentLoaded)`；不要同时保留一个绕过必需依赖的模板构造函数。参见[自定义控件](/zh/plugin/control)。
