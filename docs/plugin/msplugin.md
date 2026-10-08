# Plugin Resource Paths

Use `ms-plugin://{assembly name}/{file path}` for plugin resources. The assembly name excludes `.dll` and must match `DllName`.

## Use in code

```csharp
using CustomExtensions.WinUI;

string original = "ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg";
string resolved = original.PluginPath();
```

`PluginPath()` returns a WinUI resource address string, not necessarily a disk path suitable for `File.ReadAllText`. The plugin must already be registered with `ApplicationExtensionHost`. Strings without the `ms-plugin://` prefix are returned unchanged.

## Use in XAML

For fixed addresses, use markup extensions from `CustomExtensions.WinUI`:

```xml
<UserControl
    x:Class="ShadowExample.Plugin.Emoji.Controls.UserControl1"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:cw="using:CustomExtensions.WinUI">
    <StackPanel>
        <Image Source="{cw:PluginImageSource Source='ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg'}" />
        <TextBlock Text="{cw:PluginPath Source='ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg'}" />
        <BitmapIcon
            UriSource="{cw:PluginUri Source='ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg'}"
            ShowAsMonochrome="False" />
    </StackPanel>
</UserControl>
```

`PluginPath` returns `string`, `PluginUri` returns `Uri`, and `PluginImageSource` returns an image source. Initialize the control with `LoadComponent` as described in [Custom Controls](/plugin/control).

For binding, register converters in control or application resources using the same `cw` namespace:

```xml
<UserControl.Resources>
    <cw:PluginPathConverter x:Key="PluginPathConverter" />
    <cw:PluginUriConverter x:Key="PluginUriConverter" />
    <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
</UserControl.Resources>
```

For a control with a `string ImagePath` property, for example, use `Source="{x:Bind ImagePath, Converter={StaticResource PluginImageSourceConverter}}"`. Converters return the same types as their corresponding markup extensions.
