# Plugin Configuration

Use [ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig) to remember a user's preferred sizes, themes, or other settings. Here we'll save the Emoji plugin's settings as JSON.

## Write a configuration class

Add `EmojiConfig.cs` to the plugin project:

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

`FileName` sets the filename, `FileExt` its extension, and `DirPath` the settings directory. Give each plugin its own filename.

`[ObservableConfigProperty]` generates a bindable property for each field. For example, `_defaultEmojiSize` becomes `DefaultEmojiSize`. The tool also generates configuration loading and saving methods.

Prefer YAML? Change `FileExt` to `.yaml`. The app needs the matching JSON/YAML loader; the earlier [initialization example](/init/customloaderclass) sets up both.

## Read settings in the plugin

The loader reads configuration before creating the plugin. Add an `[Autowired]` property to receive it:

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

Declare the configuration class as `public partial` and set `FileName`, as shown above. Nested configuration without a filename belongs inside another configuration object and isn't loaded as a separate file.

## Bind settings to the UI

Controls can also receive `EmojiConfig` through injection, or call `DiFactory.Services.Resolve<EmojiConfig>()` after the plugin has loaded. This lets the plugin and its controls share the same settings object.

If the control exposes it through a `ViewModel` property, bind it like this:

```xml
<NumberBox Header="Emoji size"
           Value="{x:Bind ViewModel.DefaultEmojiSize, Mode=TwoWay}" />
<CheckBox Content="Auto complete"
          IsChecked="{x:Bind ViewModel.EnableAutoComplete, Mode=TwoWay}" />
```

See the ShadowObservableConfig documentation for nested settings, automatic saving, and custom formats.
