# 自定义控件

## WinUI控件规则

创建新的`Page`或者`用户控件`的时候会自动生成一个构造函数

```csharp
// 示例
public LoginTip()
{
    this.InitializeComponent();
}
```
但是在插件中默认的`InitializeComponent()`无法正常加载

所以我们要改为
```csharp
using CustomExtensions.WinUI; // [!code ++]
public LoginTip()
{
    this.InitializeComponent(); // [!code --]
    this.LoadComponent(ref _contentLoaded); // [!code ++]
}
```

这样就能正常识别与载入插件的Xaml内容

::: warning 注意

插件中的每一个`Page`或者`UserControl/Control`都要更改为这种形式,否则将无法加载

:::

## 编写

其余用法与普通WinUI项目一致

