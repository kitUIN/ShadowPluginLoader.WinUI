# Custom Controls

You can write `Page` and `UserControl` classes in plugins just as you would in any WinUI project. The part to change is how the control loads its XAML.

After creating a control, open its `.xaml.cs` file and replace `InitializeComponent()` in the constructor with `LoadComponent`:

```csharp [UserControl1.xaml.cs]
using CustomExtensions.WinUI;
using Microsoft.UI.Xaml.Controls;

namespace ShadowExample.Plugin.Emoji.Controls;

public sealed partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        this.LoadComponent(ref _contentLoaded);
    }
}
```

This lets the control find its XAML inside the plugin. WinUI already generates `_contentLoaded`, so you don't need to declare it.

Create controls after the plugin has loaded, and keep only the `LoadComponent` call shown above. Controls written entirely in code don't need this step.

If you generate constructors with `[Autowired]`, put the call in `partial void ConstructorInit()`; see [Quick Dependency Injection](/advance/quickdi). For images in the UI, see [Plugin Resource Paths](/plugin/msplugin).
