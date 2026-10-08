# 自定义资源字典

平时我们会把颜色、样式放进资源字典，再合并到 `App.xaml`。插件没有自己的 `App.xaml`，可以让插件主类帮忙加载这些字典。

在主类中加上下面的属性，并引用 `System.Collections.Generic`：

```csharp
protected override IEnumerable<string> ResourceDictionaries =>
    ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1.xaml"];
```

这里的 `ShadowExample.Plugin.Emoji` 是 DLL 的程序集名称。插件创建时，加载器会把列出的字典合并到应用资源中，之后就可以用 `StaticResource` 引用里面的颜色和样式了。

如果有多个字典，继续往列表里添加路径即可。字典内容的写法和普通 WinUI 项目一样。

字典会在插件加载时合并，禁用插件后也会保留，所以资源名最好带上插件前缀，避免和其他插件重名。随主程序一起部署的内置资源可以使用 `ms-appx:///` 路径。

路径的更多写法见[插件资源路径](/zh/plugin/msplugin)。
