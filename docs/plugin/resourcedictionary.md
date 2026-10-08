# Custom Resource Dictionaries

In a normal app, you might put colors and styles in a resource dictionary and merge it in `App.xaml`. Plugins don't have their own `App.xaml`, so the main plugin class can load these dictionaries for you.

Add this property to the main class and import `System.Collections.Generic`:

```csharp
protected override IEnumerable<string> ResourceDictionaries =>
    ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1.xaml"];
```

`ShadowExample.Plugin.Emoji` is the DLL's assembly name. When the plugin is created, these dictionaries are merged into the app's resources. You can then use `StaticResource` to reference their colors and styles.

For more dictionaries, add more paths to the list. Write their contents just as you would in a normal WinUI project.

Dictionaries stay in the app's resources when a plugin is disabled. Prefix your resource names with the plugin name to avoid clashes. Built-in resources deployed with the app can use `ms-appx:///` paths.

See [Plugin Resource Paths](/plugin/msplugin) for more examples.
