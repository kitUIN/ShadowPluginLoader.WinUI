# 自定义控件

在插件中创建带 XAML 的 `Page` 或 `UserControl` 后，将模板构造函数里的 `InitializeComponent()` 替换为扩展加载方法：

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

`_contentLoaded` 是 WinUI XAML 生成代码提供的字段，不需要重复声明。`LoadComponent` 根据控件所在程序集和调用文件定位 XAML，再调用 `Application.LoadComponent`。

确保对应程序集已经通过插件加载流程注册到扩展宿主，再创建控件。不要同时调用 `InitializeComponent()` 和 `LoadComponent()`。纯代码控件不需要增加这段 XAML 加载逻辑。

如果控件使用 `[Autowired]` 自动生成构造函数，将加载调用放到 `partial void ConstructorInit()`，详见[快速依赖注入](/zh/advance/quickdi)。图片和其他插件资源使用[插件资源路径](/zh/plugin/msplugin)。
