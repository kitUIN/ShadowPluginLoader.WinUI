# 插件配置文件

想让插件记住用户选的字号、主题或其他设置，可以使用 [ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig)。这里用 JSON 保存 Emoji 插件的设置。

## 写一个配置类

在插件项目中添加 `EmojiConfig.cs`：

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

`FileName` 是文件名，`FileExt` 是扩展名，`DirPath` 是保存配置的目录。不同插件最好使用不同文件名。

给字段加上 `[ObservableConfigProperty]` 后，工具会生成可绑定的属性，比如 `_defaultEmojiSize` 对应 `DefaultEmojiSize`，也会生成加载和保存配置的方法。

如果更喜欢 YAML，把 `FileExt` 改成 `.yaml`。主程序需要先设置好对应的 JSON/YAML 加载器，前面的[初始化示例](/zh/init/customloaderclass)已经包含了这一步。

## 在插件里读取设置

加载器会先加载配置，再创建插件。所以在插件主类里添加一个 `[Autowired]` 属性，就能拿到配置：

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

配置类要像示例这样声明为 `public partial`，并填写 `FileName`。没有文件名的嵌套配置可以放在其他配置对象里，不会被单独当作文件加载。

## 绑定到界面

控件也可以注入 `EmojiConfig`，或在插件加载完成后调用 `DiFactory.Services.Resolve<EmojiConfig>()`。这样主类和控件用到的是同一份配置。

假设控件把它放在 `ViewModel` 属性中，就可以这样绑定：

```xml
<NumberBox Header="Emoji size"
           Value="{x:Bind ViewModel.DefaultEmojiSize, Mode=TwoWay}" />
<CheckBox Content="Auto complete"
          IsChecked="{x:Bind ViewModel.EnableAutoComplete, Mode=TwoWay}" />
```

更多设置方式，比如嵌套配置、自动保存和自定义格式，可以查看 ShadowObservableConfig 的文档。
