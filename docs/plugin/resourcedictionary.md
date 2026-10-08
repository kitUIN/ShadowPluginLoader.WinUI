# Custom Resource Dictionaries

Plugins do not have their own `App.xaml`. Override the **protected** `ResourceDictionaries` property in the main plugin class, with `using System.Collections.Generic;`:

```csharp
protected override IEnumerable<string> ResourceDictionaries =>
    ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1.xaml"];
```

`ShadowExample.Plugin.Emoji` is the assembly name, not a display name. `AbstractPlugin<TMeta>.Init()` resolves each path, creates a `ResourceDictionary`, and adds it to `Application.Current.Resources.MergedDictionaries`.

Merging happens during instance construction, before `Loaded()` and enabled events. Dictionaries are merged even for plugins with persisted disabled state and are not automatically removed when disabling a plugin. Avoid conflicting resource keys across plugins.

Built-in resources deployed with the host can use their correct `ms-appx:///` paths. Dictionary XAML otherwise follows ordinary WinUI usage; cross-plugin resource paths must follow the [resource path rules](/plugin/msplugin).
