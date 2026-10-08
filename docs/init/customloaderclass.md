# Create the Loader and Connect It to Your App

Now that you have metadata and a plugin base class, let's connect them to a loader and use it in your app.

## Create the loader

Add `ShadowExamplePluginLoader.cs` to the SDK:

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

The two type arguments are your metadata and plugin base class. `[CheckAutowired]` generates the constructor, so you don't need any code in the class yet. Keep the `partial` keyword.

## Initialize it in the app

Reference the same SDK from your app, then add this initialization to the constructor in `App.xaml.cs`. Keep your existing `OnLaunched` and window creation code.

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

This prepares XAML loading for plugins, sets up JSON/YAML configuration, initializes dependency injection, and registers your loader.

The `events` registration lets the app and plugins share one event service. The two `plans` assignments prepare empty upgrade and removal lists for the first launch. Follow the order shown above.

The type argument order is easy to mix up: the loader takes `<ExampleMetaData, PluginBase>`, while `DiFactory.Init` takes `<PluginBase, ExampleMetaData>`.

## Next: load a plugin

In your window, call `DiFactory.Services.Resolve<ShadowExamplePluginLoader>()` to get the loader. Await `CheckUpgradeAndRemoveAsync()` before loading plugins.

Loading uses WinUI resources, so call it on the window's UI thread. See [Install, Update, and Remove](/plugin/install) for a complete example. To add your own work before or after loading, see [Custom Loading Logic](/advance/customloadplugin).
