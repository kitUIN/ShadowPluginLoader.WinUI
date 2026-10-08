# Plugin Resource Paths

The app needs to know which plugin owns an image before it can find it. Use `ms-plugin://{assembly name}/{file path}` to identify resources inside a plugin.

For example, `Assets/th.jpg` in Emoji becomes `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`. Leave `.dll` off the assembly name.

## Use a path in code

Once the plugin has loaded, call `PluginPath()` to turn the address into a resource URI WinUI understands:

```csharp
using CustomExtensions.WinUI;

string original = "ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg";
string resolved = original.PluginPath();
```

Pass the result to WinUI controls. For disk access with methods such as `File.ReadAllText`, use the file's actual filesystem path.

## Use a path in XAML

Import `CustomExtensions.WinUI`, then choose the extension that matches your control's property:

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

- `PluginImageSource` works with image properties such as `Image.Source`.
- `PluginUri` returns a `Uri` for properties such as `BitmapIcon.UriSource`.
- `PluginPath` returns a string, useful for displaying the resource address.

Also update the control's constructor to use `LoadComponent`, as shown in [Custom Controls](/plugin/control).

## Use a path with binding

For a bound address, use the matching converter. First add the converters to control or application resources:

```xml
<UserControl.Resources>
    <cw:PluginPathConverter x:Key="PluginPathConverter" />
    <cw:PluginUriConverter x:Key="PluginUriConverter" />
    <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
</UserControl.Resources>
```

If your control has a `string ImagePath` property, set the image's source to `Source="{x:Bind ImagePath, Converter={StaticResource PluginImageSourceConverter}}"`.

The other two converters work the same way, returning a string or a `Uri`.
