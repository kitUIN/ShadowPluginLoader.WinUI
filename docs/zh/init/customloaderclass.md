# 创建加载器并接入主程序

插件信息和基类都有了，接下来把它们交给加载器，再让主程序使用这个加载器。

## 创建加载器

在 SDK 中新建 `ShadowExamplePluginLoader.cs`：

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

这里的两个类型分别是插件元数据和插件基类。`[CheckAutowired]` 会帮我们生成构造函数，所以暂时不用往类里写其他代码。记得保留 `partial`。

## 在主程序中初始化

让主程序引用同一个 SDK，然后在 `App.xaml.cs` 的构造函数中加入下面的初始化代码。原有的 `OnLaunched` 和创建窗口的代码继续保留。

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

这段代码依次做了几件事：准备插件的 XAML 加载环境，设置 JSON/YAML 配置支持，初始化依赖注入，最后注册我们刚写好的加载器。

中间的 `events` 注册让主程序和插件共用同一个事件服务；`plans` 的两行则为首次启动准备好升级和删除列表。按示例顺序放好即可。

这里容易写反的是泛型参数：加载器是 `<ExampleMetaData, PluginBase>`，而 `DiFactory.Init` 是 `<PluginBase, ExampleMetaData>`。

## 下一步：加载插件

在窗口中调用 `DiFactory.Services.Resolve<ShadowExamplePluginLoader>()` 就能拿到加载器。之后先等待 `CheckUpgradeAndRemoveAsync()` 完成，再开始加载插件。

加载过程会用到 WinUI 资源，放在窗口的 UI 线程中调用就好。完整示例见[安装、更新和删除](/zh/plugin/install)。如果想在加载前后加上自己的处理，可以看[自定义加载逻辑](/zh/advance/customloadplugin)。
