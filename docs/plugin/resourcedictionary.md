# Custom Resource Dictionary

## Resource Dictionary Rules

Generally, we put some commonly used `Color` and `Style` into a `theme.xaml`.

This `theme.xaml` internally contains `ResourceDictionary`.

In normal projects, we would put it in `App.xaml`, but plugins don't have `App.xaml`, so this functionality is implemented in the `AbstractPlugin` class.

## Example

Assuming in plugin `ShadowViewer.Plugin.Bika` (DLL name)

We have a file at `/Themes/BikaTheme.xaml`

Please implement the following accessor in your own plugin main class:

```csharp
public override IEnumerable<string> ResourceDictionaries => new List<string>
{
    "ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml"
};
```

This will automatically merge the resource dictionary into `App.xaml` when the plugin is loaded.

::: tip Note

If it's a built-in plugin, just use the `ms-appx:///` path directly.

:::

## Development

The rest of the usage is consistent with regular WinUI projects.
