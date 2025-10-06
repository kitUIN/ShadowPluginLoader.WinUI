# 自定义资源字典

## 资源字典规则

一般来说,我们会将一些常用的`Color`,`Style`塞到一个`theme.xaml`中

这个`theme.xaml`内部是`ResourceDictionary`

正常项目中我们都会塞到`App.xaml`里,但是插件是没有`App.xaml`的,所以在`AbstractPlugin`类中就实现了该功能


## 示例

假设在插件`ShadowViewer.Plugin.Bika`(DLL名称)中

我们有一个文件在`/Themes/BikaTheme.xaml`

请用你自己的插件主类实现以下访问器

```csharp
public override IEnumerable<string> ResourceDictionaries => new List<string>
{
    "ms-plugin://ShadowViewer.Plugin.Bika/Themes/BikaTheme.xaml"
};
```

这样会在插件加载进入的时候资源字典会自动合并到`App.xaml`

::: tip 说明

如果是内置插件,直接使用`ms-appx:///`路径即可

:::

## 编写

其余用法与普通WinUI项目一致