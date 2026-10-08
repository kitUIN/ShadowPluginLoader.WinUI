# Quick Dependency Injection

`ShadowPluginLoader.SourceGenerator` generates constructors; DryIoc resolves the dependencies. Declare participating classes as `public partial`. Attributes are in `ShadowPluginLoader.Attributes`.

## Autowired properties

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

The generated constructor is equivalent to:

```csharp
public StatusViewModel(ILogger logger)
{
    Logger = logger;
    ConstructorInit();
}
```

The generator also declares `partial void ConstructorInit()`. Implement it in your class for initialization after dependency assignment. Do not add another constructor with the same signature.

## CheckAutowired classes

Mark derived classes with `[CheckAutowired]` to detect and forward base constructor parameters even without new `[Autowired]` properties. For example, the [main plugin class](/plugin/create) receives:

```csharp
public EmojiPlugin(ExampleMetaData meta, ILogger logger,
    PluginEventService pluginEventService)
    : base(meta, logger, pluginEventService)
{
    ConstructorInit();
}
```

These fragments explain generated output; do not paste them into classes that already receive generated constructors. `[CheckAutowired]` on an empty class does not invent service parameters. If there are no injection parameters, no constructor is generated.

## Service registration

Attributes do not register application services. Register dependencies with methods such as `DiFactory.Services.Register<TService, TImplementation>()` before resolving consumers. The default main processor registers plugin main classes by plugin ID and supplies their metadata. Plugins do not need to register their own main classes. Public configuration classes are loaded and registered as described in [Plugin Configuration](/plugin/config).

For controls using generated constructors, call `this.LoadComponent(ref _contentLoaded)` inside `ConstructorInit()`. Avoid retaining a template constructor that bypasses required dependencies. See [Custom Controls](/plugin/control).
