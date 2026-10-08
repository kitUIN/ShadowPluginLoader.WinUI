# 插件资源路径

插件资源地址格式为 `ms-plugin://{程序集名称}/{文件路径}`。这里使用不含 `.dll` 的程序集名称，保持与 `DllName` 一致。

## 代码中使用

```csharp
using CustomExtensions.WinUI;

string original = "ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg";
string resolved = original.PluginPath();
```

`PluginPath()` 返回供 WinUI 使用的资源地址字符串，不保证是可直接传给 `File.ReadAllText` 的磁盘路径。它需要插件已注册到 `ApplicationExtensionHost`；非 `ms-plugin://` 字符串保持原样。

## 在 XAML 中使用

静态资源地址可使用 `CustomExtensions.WinUI` 中的标记扩展：

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

`PluginPath` 返回 `string`，`PluginUri` 返回 `Uri`，`PluginImageSource` 返回图片源。控件代码还需按[自定义控件](/zh/plugin/control)调用 `LoadComponent`。

对于数据绑定，先在控件或应用资源中注册转换器（沿用上面的 `cw` 命名空间）：

```xml
<UserControl.Resources>
    <cw:PluginPathConverter x:Key="PluginPathConverter" />
    <cw:PluginUriConverter x:Key="PluginUriConverter" />
    <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
</UserControl.Resources>
```

例如控件有一个 `string ImagePath` 属性时，可使用 `Source="{x:Bind ImagePath, Converter={StaticResource PluginImageSourceConverter}}"`。转换器与对应标记扩展的返回类型相同。
