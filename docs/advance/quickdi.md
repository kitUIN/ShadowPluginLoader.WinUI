# Quick Dependency Injection

Writing a constructor for every new service gets repetitive. `[Autowired]` and `[CheckAutowired]` can generate that code for you.

Declare the class as `public partial` and import `ShadowPluginLoader.Attributes`.

## Add Autowired to a property

For example, this ViewModel needs a logger:

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

The build generates a constructor like this:

```csharp
public StatusViewModel(ILogger logger)
{
    Logger = logger;
    ConstructorInit();
}
```

After assigning `Logger`, it calls `ConstructorInit()`. Put any extra setup there instead of writing another constructor.

## What if the base class needs parameters too?

Add `[CheckAutowired]` to the derived class. The generator will pass along the parameters its base class needs. For the earlier [main plugin class](/plugin/create), the constructor looks like this:

```csharp
public EmojiPlugin(ExampleMetaData meta, ILogger logger,
    PluginEventService pluginEventService)
    : base(meta, logger, pluginEventService)
{
    ConstructorInit();
}
```

This code is generated for you. You don't need to paste it into the plugin class.

## Register your services

DryIoc still needs to know where each service comes from. Register your own services first with methods such as `DiFactory.Services.Register<TService, TImplementation>()`.

The loader registers plugin main classes and supported [configuration classes](/plugin/config) for you.

Controls can receive dependencies this way too. Put `this.LoadComponent(ref _contentLoaded)` in `ConstructorInit()` to load XAML once the dependencies are ready; see [Custom Controls](/plugin/control).
