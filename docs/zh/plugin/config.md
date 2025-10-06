# 插件配置文件

> 依赖于[ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig)


`FileExt` 可选: `.yaml` 或 `.json`

### 1. 创建配置类(以yaml为例子)

```csharp
using ShadowObservableConfig.Attributes;
using System.Collections.ObjectModel;

[ObservableConfig(FileName = "app_config", FileExt = ".yaml", DirPath = "config", Description = "应用程序配置", Version = "1.0.0")]
public partial class AppConfig
{
    [ObservableConfigProperty(Name = "AppName", Description = "应用程序名称")]
    private string _appName = "My App";

    [ObservableConfigProperty(Name = "IsEnabled", Description = "是否启用")]
    private bool _isEnabled = true;

    [ObservableConfigProperty(Name = "MaxRetryCount", Description = "最大重试次数")]
    private int _maxRetryCount = 3;

    [ObservableConfigProperty(Name = "Settings", Description = "应用设置")]
    private AppSettings _settings = new();

    [ObservableConfigProperty(Name = "Features", Description = "功能列表")]
    private ObservableCollection<string> _features = new();
}

[ObservableConfig(Description = "应用设置", Version = "1.0.0")]
public partial class AppSettings
{
    [ObservableConfigProperty(Name = "Theme", Description = "主题")]
    private string _theme = "Light";

    [ObservableConfigProperty(Name = "Language", Description = "语言")]
    private string _language = "zh-CN";
}
```

### 2. 在 WinUI 3 中使用(以yaml为例子)

```csharp
// App.xaml.cs
public App()
{
    InitializeComponent();
    ShadowObservableConfig.GlobalSetting.Init(ApplicationData.Current.LocalFolder.Path,
    [
        new ShadowObservableConfig.Yaml.YamlConfigLoader()
    ]);
}
```


```csharp
public sealed partial class MainPage : Page
{
    public AppConfig ViewModel { get; } = AppConfig.Load();

    public MainPage()
    {
        this.InitializeComponent();
        ViewModel.ConfigChanged += OnConfigChanged;
    }

    private void OnConfigChanged(object? sender, ConfigChangedEventArgs e)
    {
        Debug.WriteLine($"配置项 '{e.FullPropertyPath}' 已更改: {e.OldValue} -> {e.NewValue}");
    }
}
```

### 3. XAML 数据绑定

```xml
<Page x:Class="MyApp.MainPage">
    <StackPanel>
        <TextBox Header="应用程序名称" 
                 Text="{x:Bind ViewModel.AppName, Mode=TwoWay}" />
        
        <CheckBox Content="启用应用程序" 
                  IsChecked="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}" />
        
        <NumberBox Header="最大重试次数" 
                   Value="{x:Bind ViewModel.MaxRetryCount, Mode=TwoWay}" />
        
        <ComboBox Header="主题" 
                  SelectedItem="{x:Bind ViewModel.Settings.Theme, Mode=TwoWay}">
            <ComboBoxItem Content="Light" />
            <ComboBoxItem Content="Dark" />
        </ComboBox>
    </StackPanel>
</Page>
```

## 📚 详细文档

### 属性说明

#### ObservableConfigAttribute
- `FileName`: 配置文件名（不含扩展名）不填该项说明当前类是内部类
- `FileExt`: 配置文件扩展名
- `DirPath`: 配置文件目录（默认为 "config"）
- `Description`: 配置描述
- `Version`: 配置版本

#### ObservableConfigPropertyAttribute
- `Name`: 属性在配置文件中的名称
- `Description`: 属性描述
- `Alias`: 属性别名(只在yaml有效)
- `AutoSave`: 是否自动保存（默认为 true）

### 支持的数据类型

- 基本类型：`string`, `int`, `double`, `bool`, `DateTime`等
- 枚举类型：任何 `enum` 类型
- 集合类型：`ObservableCollection<T>`
- 嵌套对象：其他标记了 `[ObservableConfig]` 的类

### 自动生成的方法

源代码生成器会自动为每个配置类生成：

- 公共属性访问器
- `Load()` 静态方法
- `Save()` 方法
- `AfterConfigInit()` 部分方法（可重写）

## 🔧 高级用法

### 自定义配置加载器

```csharp
public class CustomConfigLoader : IConfigLoader
{
    public T Load<T>(string filePath) where T : class
    {
        // 自定义加载逻辑
        return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
    }

    public void Save<T>(T config, string filePath) where T : class
    {
        // 自定义保存逻辑
        File.WriteAllText(filePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```

自定义结束记得在`ShadowObservableConfig.GlobalSetting.Init`里设置

### 配置初始化回调

```csharp
[ObservableConfig(FileName = "my_config")]
public partial class MyConfig
{
    [ObservableConfigProperty(Name = "Value")]
    private string _value = "default";

    partial void AfterConfigInit()
    {
        // 配置加载完成后的初始化逻辑
        Console.WriteLine($"配置已加载: {Value}");
    }
}
```