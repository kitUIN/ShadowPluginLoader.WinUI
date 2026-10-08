# 插件配置文件

配置由 [ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig) 提供。当前加载器引用其 JSON 和 YAML 实现。

## 定义配置

```csharp [EmojiConfig.cs]
using ShadowObservableConfig.Attributes;

namespace ShadowExample.Plugin.Emoji;

[ObservableConfig(FileName = "emoji_config", FileExt = ".json", DirPath = "config")]
public partial class EmojiConfig
{
    [ObservableConfigProperty]
    private int _defaultEmojiSize = 24;

    [ObservableConfigProperty]
    private bool _enableAutoComplete = true;
}
```

源生成器生成可观察属性及配置加载/保存支持。`FileExt` 可使用 `.json` 或 `.yaml`，宿主必须先注册对应的配置加载器；SDK 自身也需要 JSON 支持，见[宿主初始化](/zh/init/customloaderclass)。为不同插件选择不同文件名，避免共用同一配置文件。

## 自动载入与注入

默认主处理器在实例化插件前扫描插件程序集的公开类型。对非抽象、继承生成的 `BaseConfig`、且 `[ObservableConfig]` 中 `FileName` 非空的类，反射调用静态 `Load()`，再按具体类型注册返回的实例。

因此可将[创建插件](/zh/plugin/create)中的主类替换为：

```csharp [EmojiPlugin.cs]
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[MainPlugin]
[CheckAutowired]
public partial class EmojiPlugin : PluginBase
{
    [Autowired]
    public EmojiConfig Config { get; }

    public override string DisplayName => "EmojiPlugin";

    partial void ConstructorInit()
    {
        Logger.Information("Emoji size: {Size}", Config.DefaultEmojiSize);
    }
}
```

不带文件名的嵌套配置不会被扫描为独立文件。配置加载错误会写入日志；若之后构造函数无法解析配置类型，应先检查这些日志。

## 在控件中使用

通过 DI 注入 `EmojiConfig`，或在自动加载完成后用 `DiFactory.Services.Resolve<EmojiConfig>()` 获取同一实例。将它暴露为控件的 `ViewModel` 属性后，可以绑定生成的属性：

```xml
<NumberBox Header="Emoji size"
           Value="{x:Bind ViewModel.DefaultEmojiSize, Mode=TwoWay}" />
<CheckBox Content="Auto complete"
          IsChecked="{x:Bind ViewModel.EnableAutoComplete, Mode=TwoWay}" />
```

在加载器管理的插件中优先复用 DI 实例。若单独使用 `EmojiConfig.Load()`，仍应先初始化 `GlobalSetting`。属性变更的保存、嵌套配置和自定义序列化规则请参阅与当前包版本对应的 ShadowObservableConfig 文档。
