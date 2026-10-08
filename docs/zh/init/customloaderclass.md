# 创建加载器并初始化宿主

## SDK 中的加载器

`[CheckAutowired]` 为 `partial` 类生成构造函数，转发基类依赖。

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

注意泛型顺序：加载器是 `<ExampleMetaData, PluginBase>`，初始化方法是 `DiFactory.Init<PluginBase, ExampleMetaData>()`。

## 宿主初始化

宿主引用同一个 SDK，在 `App.xaml.cs` 中完成以下初始化。保留 WinUI 模板已有的 `OnLaunched` 和窗口创建代码。

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

顺序很重要：先初始化扩展宿主和配置加载器，再初始化 DI，最后注册并解析插件加载器。JSON 加载器用于 SDK 自身配置，YAML 加载器供使用 YAML 的插件配置使用。

当前 `DiFactory` 只将事件服务注册为 `IPluginEventService`，而插件与加载器构造函数依赖具体 `PluginEventService`。上面的注册让二者共用同一实例。当前 `InnerSdkConfig` 的计划集合没有字段初始值，因此首次启动时也显式补上空集合。

在窗口中用 `DiFactory.Services.Resolve<ShadowExamplePluginLoader>()` 获取加载器。启动时先 `await loader.CheckUpgradeAndRemoveAsync()`，再使用流水线加载插件，见[安装与管理](/zh/plugin/install)。加载会创建 WinUI 资源，应从 UI 线程发起并保留其同步上下文。

扩展钩子见[自定义加载逻辑](/zh/advance/customloadplugin)。
