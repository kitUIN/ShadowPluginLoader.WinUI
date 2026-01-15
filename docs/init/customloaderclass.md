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

The following is a [DiFactory.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/DIFactory.cs) already created in this system. For more information, see the `DryIoc` documentation.

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
    DiFactory.RegisterPluginLoader<PluginLoader>();
}
```

## Custom Plugin Loading Process

Sometimes we want to customize the plugin loading process, such as performing some verification before loading plugins.

We can override the default loading logic.

For details, see [Custom Plugin Loading Logic](/advance/customloadplugin)
