# Create the Loader and Initialize the Host

## Loader in the SDK

`[CheckAutowired]` generates a constructor for the `partial` class and forwards base dependencies.

```csharp [ShadowExamplePluginLoader.cs]
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core;

[CheckAutowired]
public partial class ShadowExamplePluginLoader
    : AbstractPluginLoader<ExampleMetaData, PluginBase>
{
}
```

Note the generic parameter order: the loader uses `<ExampleMetaData, PluginBase>`, while initialization uses `DiFactory.Init<PluginBase, ExampleMetaData>()`.

## Host initialization

Reference the same SDK from the host and initialize it in `App.xaml.cs`. Retain the WinUI template's `OnLaunched` and window creation code.

```csharp [App.xaml.cs]
using CustomExtensions.WinUI;
using DryIoc;
using Microsoft.UI.Xaml;
using ShadowExample.Core;
using ShadowExample.Core.Plugins;
using ShadowObservableConfig.Json;
using ShadowObservableConfig.Yaml;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Services;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace ShadowExample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ApplicationExtensionHost.Initialize(this);
        ShadowObservableConfig.GlobalSetting.Init(
            ApplicationData.Current.LocalFolder.Path,
            [new JsonConfigLoader(), new YamlConfigLoader()]);

        DiFactory.Init<PluginBase, ExampleMetaData>();

        var events = (PluginEventService)DiFactory.Services.Resolve<IPluginEventService>();
        DiFactory.Services.RegisterInstance<PluginEventService>(events);

        var plans = DiFactory.Services.Resolve<InnerSdkConfig>();
        plans.PlanRemove ??= new ObservableCollection<PlanRemoveData>();
        plans.PlanUpgrade ??= new ObservableCollection<PlanUpgradeData>();

        DiFactory.RegisterPluginLoader<ShadowExamplePluginLoader>();
    }
}
```

Initialize the extension host and configuration loaders before DI, then register and resolve the plugin loader. JSON support is needed for SDK configuration; YAML support is used by plugins with YAML configuration.

The current `DiFactory` registers only `IPluginEventService`, while plugin and loader constructors request concrete `PluginEventService`. The registration above shares the same instance between both types. Current `InnerSdkConfig` plan collection fields have no initializers, so empty collections are supplied for the first launch as well.

Resolve the loader in the window with `DiFactory.Services.Resolve<ShadowExamplePluginLoader>()`. At startup, await `loader.CheckUpgradeAndRemoveAsync()` before processing plugins; see [Installation and Management](/plugin/install). Loading creates WinUI resources, so start it on the UI thread and preserve its synchronization context.

See [Custom Loading Logic](/advance/customloadplugin) for extension hooks.
