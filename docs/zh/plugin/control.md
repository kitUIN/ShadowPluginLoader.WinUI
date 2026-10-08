# 自定义控件

插件里也可以写 `Page` 和 `UserControl`，界面写法和普通 WinUI 项目一样。需要改的是加载 XAML 的那一步。

新建控件后，打开它的 `.xaml.cs` 文件，把构造函数里的 `InitializeComponent()` 换成 `LoadComponent`：

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

这样控件就能找到插件里的 XAML 文件了。`_contentLoaded` 已经由 WinUI 生成，不用自己再声明。

在插件加载完成后再创建控件，并且只保留上面的 `LoadComponent` 调用。如果是没有 XAML 的纯代码控件，就不需要这一步。

如果你用 `[Autowired]` 生成构造函数，把这行调用放进 `partial void ConstructorInit()` 即可，具体见[快速依赖注入](/zh/advance/quickdi)。界面里的图片可以按[插件资源路径](/zh/plugin/msplugin)来引用。
