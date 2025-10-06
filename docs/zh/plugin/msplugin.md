# 插件资源路径

## 定义

`ms-plugin://{程序集名称}/{文件路径}`

## 示例

程序集名称: `ShadowViewer.Plugin.Bika`

文件路径: `/Themes/BikaTheme.xaml`

插件资源路径: `ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml`

## 使用

### 代码中使用

```csharp
using CustomExtensions.WinUI;
public void Test()
{
    string originPath =  "ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml"
    string realPath = originPath.PluginPath();
}
```

### 在 XAML 中使用

#### Converter

这里提供三种 Converter

```xml [App.xaml]
<!-- xmlns:cw="using:CustomExtensions.WinUI" -->
<cw:PluginPathConverter x:Key="PluginPathConverter" />
<cw:PluginUriConverter x:Key="PluginUriConverter" />
<cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
```

- `PluginPathConverter` 用于返回值类型为 string
- `PluginUriConverter` 用于返回值类型为 Uri
- `PluginImageSourceConverter` 用于返回值类型为 ImageSource

使用时需要在 App.xaml 或者 Page 中实例化

::: code-group

```xml [App.xaml]
<Application
    x:Class="ShadowViewer.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:cw="using:CustomExtensions.WinUI"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <cw:PluginUriConverter x:Key="PluginUriConverter" />
            <cw:PluginPathConverter x:Key="PluginPathConverter" />
            <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

```xml [Page.xaml]
<Page.Resources>
    <cw:PluginUriConverter x:Key="PluginUriConverter" />
    <cw:PluginPathConverter x:Key="PluginPathConverter" />
    <cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
</Page.Resources>
```

:::

使用`x:Bind`

```xml
<local2:PluginLogo
    Grid.Column="0"
    Width="60"
    Height="60"
    FontIconSize="35"
    FontSize="40"
    Logo="{x:Bind MetaData.Logo, Mode=OneWay, Converter={StaticResource PluginPathConverter}}" />
```

#### Extension

这里提供三种 Extension

```xml
<!-- xmlns:cw="using:CustomExtensions.WinUI" -->
cw:PluginPath
cw:PluginUri
cw:PluginImageSource
```

```xml
<!-- xmlns:cw="using:CustomExtensions.WinUI" -->

<Image
    Grid.Row="0"
    Width="300"
    Source="{cw:PluginImageSource Source='ms-plugin://ShadowViewer.Plugin.Bika/Assets/Picacgs/logo.png'}"
    />
<TextBlock
    Width="30"
    Height="30"
    Text="{cw:PluginPath Source='ms-plugin://ShadowViewer.Plugin.Bika/Assets/Icons/logo.png'}"
     />
<BitmapIcon
    Width="30"
    Height="30"
    UriSource="{cw:PluginUri Source='ms-plugin://ShadowViewer.Plugin.Bika/Assets/Icons/logo.png'}"
    ShowAsMonochrome="False" />

```
