# 插件资源路径

主程序需要知道一张图片属于哪个插件，才能找到它。为此，我们用 `ms-plugin://{程序集名称}/{文件路径}` 来写插件资源的地址。

比如 Emoji 插件中的 `Assets/th.jpg`，地址就是 `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`。程序集名称不用加 `.dll`。

## 在代码中使用

插件加载好后，调用 `PluginPath()`，就能把这个地址转换成 WinUI 能识别的资源地址：

```csharp
using CustomExtensions.WinUI;

string original = "ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg";
string resolved = original.PluginPath();
```

转换结果适合交给 WinUI 控件使用。如果要用 `File.ReadAllText` 一类方法读取磁盘文件，请使用文件的实际路径。

## 在 XAML 中使用

先引入 `CustomExtensions.WinUI` 命名空间，再根据控件需要的类型选择写法：

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

- `PluginImageSource` 用于 `Image.Source` 这样的图片属性。
- `PluginUri` 返回 `Uri`，适合 `BitmapIcon.UriSource`。
- `PluginPath` 返回字符串，可以用来显示资源地址。

控件的构造函数也要按[自定义控件](/zh/plugin/control)一节改成 `LoadComponent`。

## 配合数据绑定

如果地址来自属性绑定，就用对应的转换器。先把它们放进控件或应用的资源中：

```xml
<UserControl.Resources>
    <cw:PluginPathConverter x:Key="PluginPathConverter" />
    <cw:PluginUriConverter x:Key="PluginUriConverter" />
    <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
</UserControl.Resources>
```

假设控件有一个 `string ImagePath` 属性，可以在图片上写 `Source="{x:Bind ImagePath, Converter={StaticResource PluginImageSourceConverter}}"`。

另外两个转换器用法相同，分别返回字符串和 `Uri`。
