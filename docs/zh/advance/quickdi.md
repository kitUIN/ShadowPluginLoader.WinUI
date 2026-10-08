# 快速依赖注入

每加一个服务，都要手写构造函数来接收它，会有些重复。这里可以用 `[Autowired]` 和 `[CheckAutowired]` 帮你生成这些代码。

使用前，给类加上 `public partial`，并引用 `ShadowPluginLoader.Attributes`。

## 给属性加上 Autowired

比如这个 ViewModel 需要写日志：

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

构建时，工具会生成类似这样的构造函数：

```csharp
public StatusViewModel(ILogger logger)
{
    Logger = logger;
    ConstructorInit();
}
```

拿到 `Logger` 后，会接着调用 `ConstructorInit()`。如果还有自己的初始化代码，放在这个方法里即可，不用另外手写构造函数。

## 基类也需要参数怎么办

给派生类加上 `[CheckAutowired]`，工具会把基类需要的参数一起带上。例如前面的[插件主类](/zh/plugin/create)，生成的构造函数是这样的：

```csharp
public EmojiPlugin(ExampleMetaData meta, ILogger logger,
    PluginEventService pluginEventService)
    : base(meta, logger, pluginEventService)
{
    ConstructorInit();
}
```

这段代码会自动生成，了解它做了什么就好，不需要再复制进插件类。

## 别忘了注册服务

生成构造函数后，DryIoc 还需要知道服务从哪里来。你自己写的服务，要先用 `DiFactory.Services.Register<TService, TImplementation>()` 等方法注册。

插件主类和符合要求的[配置类](/zh/plugin/config)由加载器帮忙注册，不用重复处理。

控件也可以这样注入依赖。把 `this.LoadComponent(ref _contentLoaded)` 放进 `ConstructorInit()`，就能在依赖准备好后加载 XAML，见[自定义控件](/zh/plugin/control)。
