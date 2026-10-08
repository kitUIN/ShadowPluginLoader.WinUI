# Plugin Configuration

Configuration is provided by [ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig). The current loader references its JSON and YAML implementations.

## Define configuration

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

Its generator provides observable properties and configuration loading/saving support. Use `.json` or `.yaml` for `FileExt`, and register the matching loader in the host first. SDK configuration also needs JSON support; see [Host Initialization](/init/customloaderclass). Use distinct filenames for different plugins to avoid sharing configuration accidentally.

## Automatic loading and injection

Before instantiating a plugin, the default main processor scans public types in its assembly. For concrete types deriving from generated `BaseConfig` and marked `[ObservableConfig]` with a nonempty `FileName`, it invokes static `Load()` through reflection and registers the returned instance by concrete type.

You can therefore replace the main class from [Create a Plugin](/plugin/create) with:

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

Nested configuration without a filename is not loaded as a separate file. Configuration loading errors are logged. Check those logs if the constructor subsequently cannot resolve a configuration type.

## Use in controls

Inject `EmojiConfig`, or resolve the same instance with `DiFactory.Services.Resolve<EmojiConfig>()` after automatic loading. Expose it as the control's `ViewModel` property and bind the generated properties:

```xml
<NumberBox Header="Emoji size"
           Value="{x:Bind ViewModel.DefaultEmojiSize, Mode=TwoWay}" />
<CheckBox Content="Auto complete"
          IsChecked="{x:Bind ViewModel.EnableAutoComplete, Mode=TwoWay}" />
```

Prefer the shared DI instance in loader-managed plugins. Standalone `EmojiConfig.Load()` still requires prior `GlobalSetting` initialization. Consult the ShadowObservableConfig documentation matching your package version for persistence, nested settings, and custom serialization.
