# Plugin Resource Path

## Definition

`ms-plugin://{Assembly Name}/{File Path}`

## Example

Assembly name: `ShadowViewer.Plugin.Bika`

File path: `/Themes/BikaTheme.xaml`

Plugin resource path: `ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml`

## Usage

### In Code

```csharp
using CustomExtensions.WinUI;
public void Test()
{
    string originPath =  "ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml"
    string realPath = originPath.PluginPath();
}
```

### In XAML

#### Converter

Three converters are provided here:

```xml [App.xaml]
<!-- xmlns:cw="using:CustomExtensions.WinUI" -->
<cw:PluginPathConverter x:Key="PluginPathConverter" />
<cw:PluginUriConverter x:Key="PluginUriConverter" />
<cw:PluginImageSourceConverter x:Key="PluginImageSourceConverter" />
```

- `PluginPathConverter` for return type string
- `PluginUriConverter` for return type Uri
- `PluginImageSourceConverter` for return type ImageSource

You need to instantiate them in App.xaml or Page when using.

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

Using `x:Bind`:

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

Three extensions are provided here:

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
