# 自定义资源字典

插件没有自己的 `App.xaml`。在插件主类中覆写 **protected** 的 `ResourceDictionaries` 属性（需要 `using System.Collections.Generic;`）：

```csharp
protected override IEnumerable<string> ResourceDictionaries =>
    ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1.xaml"];
```

路径中的 `ShadowExample.Plugin.Emoji` 是程序集名称，不是显示名称。`AbstractPlugin<TMeta>.Init()` 将路径转换后创建 `ResourceDictionary` 并加入 `Application.Current.Resources.MergedDictionaries`。

合并发生在插件实例构造期间，早于 `Loaded()` 和启用事件。即使插件保存为禁用状态，字典也会合并；默认实现没有在禁用时自动移除字典。应避免插件间资源键冲突。

内置且已随宿主部署的资源可使用正确的 `ms-appx:///` 路径。普通字典内部的 XAML 写法与 WinUI 一致，但跨插件资源路径仍应遵循[资源路径规则](/zh/plugin/msplugin)。
