# Create Plugin Loader Class

::: warning Note

You should first design the [plugin metadata](/init/metaplugin) and [plugin base class](/init/iplugin), then create the loader class.

:::

## Create Plugin Loader Class

```csharp [ShadowExamplePluginLoader.cs]
using System;
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    [CheckAutowired]
    public partial class ShadowExamplePluginLoader :
        AbstractPluginLoader<ExampleMetaData, PluginBase>
    {
    }
}
```

- Here we use the `[CheckAutowired]` annotation to automatically generate the corresponding constructor
- (Optional) You can inherit and modify some methods in `AbstractPluginLoader`

## Using Dependency Injection

The plugin loading functionality is mainly implemented through dependency injection.

So we first need to create dependency injection, and must use the `DryIoc` library for dependency injection.

The following is a [DiFactory.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/DIFactory.cs) already created in this system. For more information, see the `DryIoc` documentation:

```csharp [DiFactory.cs]
using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Installer;
using ShadowPluginLoader.WinUI.Scanners;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Dependency injection factory
/// </summary>
public static class DiFactory
{
    /// <summary>
    /// Dependency injection container
    /// </summary>
    public static Container Services { get; }

    static DiFactory()
    {
        Services = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments));
        Services.Register(
            Made.Of(() => Serilog.Log.ForContext(Arg.Index<Type>(0)),
                r => r.ImplementationType ?? r.Parent.ImplementationType ?? typeof(object)),
            setup: Setup.With(condition: r => r.Parent.ImplementationType != null || r.ImplementationType != null));
        Services.Register<PluginEventService>(reuse: Reuse.Singleton);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Init<TAPlugin, TMeta>()
        where TAPlugin : AbstractPlugin<TMeta>
        where TMeta : AbstractPluginMetaData
    {
        MetaDataHelper.Init<TMeta>();
        var baseSdkConfig = BaseSdkConfig.Load();
        Services.RegisterInstance(baseSdkConfig);
        var innerSdkConfig = InnerSdkConfig.Load();
        Services.RegisterInstance(innerSdkConfig);
        Services.Register<IDependencyChecker<TMeta>, DependencyChecker<TMeta>>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IRemoveChecker, RemoveChecker>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IPluginScanner<TAPlugin, TMeta>, PluginScanner<TAPlugin, TMeta>>(
            serviceKey: "base", reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
            ); 
        Services.Register<IPluginInstaller<TMeta>, ZipPluginInstaller<TMeta>>(
            serviceKey: "base",  reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
                .OverrideWith(Parameters.Of.Type<IPluginScanner<TAPlugin, TMeta>>(serviceKey: "base"))
            );
    }
}
```

## Using in Main Project

In the `App.xaml.cs` file, some content needs to be initialized:

```csharp [App.xaml.cs]
public App()
{
    this.InitializeComponent();
    Init();
}

private void Init()
{
    ApplicationExtensionHost.Initialize(this);
    ShadowObservableConfig.GlobalSetting.Init(
        ApplicationData.Current.LocalFolder.Path,
        [
            new JsonConfigLoader(),
            new YamlConfigLoader()
        ]);
    DiFactory.Init<PluginBase, ExampleMetaData>();
    DiFactory.Services.Register<ShadowExamplePluginLoader>(
        reuse: Reuse.Singleton,
        made: Parameters.Of
            .Type<IDependencyChecker<ExampleMetaData>>(serviceKey: "base")
            .OverrideWith(Parameters.Of.Type<IRemoveChecker>(serviceKey: "base"))
            .OverrideWith(Parameters.Of.Type<IPluginScanner<PluginBase, ExampleMetaData>>(serviceKey: "base"))
            .OverrideWith(Parameters.Of.Type<IPluginInstaller<ExampleMetaData>>(serviceKey: "base"))
    );
}
```

## Custom Plugin Loading Process

Sometimes we want to customize the plugin loading process, such as performing some verification before loading plugins.

We can override the default loading logic.

For details, see [Custom Plugin Loading Logic](/advance/customloadplugin)
