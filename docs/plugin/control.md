# Custom Controls

## WinUI Control Rules

When creating a new `Page` or `UserControl`, a constructor is automatically generated.

```csharp
// Example
public LoginTip()
{
    this.InitializeComponent();
}
```

However, the default `InitializeComponent()` in plugins cannot load normally.

So we need to change it to:
```csharp
using CustomExtensions.WinUI; // [!code ++]
public LoginTip()
{
    this.InitializeComponent(); // [!code --]
    this.LoadComponent(ref _contentLoaded); // [!code ++]
}
```

This way, the plugin's Xaml content can be properly recognized and loaded.

::: warning Note

Every `Page` or `UserControl/Control` in the plugin must be changed to this form, otherwise it cannot be loaded.

:::

## Development

The rest of the usage is consistent with regular WinUI projects.
