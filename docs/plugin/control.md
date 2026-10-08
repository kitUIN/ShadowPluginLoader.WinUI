# Custom Controls

After creating a XAML-backed `Page` or `UserControl` in a plugin, replace the template constructor's `InitializeComponent()` with the extension loader:

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

WinUI-generated XAML code provides `_contentLoaded`; do not declare it again. `LoadComponent` locates XAML using the control assembly and calling file, then calls `Application.LoadComponent`.

Load the plugin assembly through the extension host before creating its controls. Do not call both `InitializeComponent()` and `LoadComponent()`. Code-only controls do not need this XAML initialization step.

For `[Autowired]` constructors, put the loading call in `partial void ConstructorInit()`; see [Quick Dependency Injection](/advance/quickdi). Use [plugin resource paths](/plugin/msplugin) for images and other resources.
