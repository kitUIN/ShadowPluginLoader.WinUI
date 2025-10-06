# Plugin Configuration File

> Depends on [ShadowObservableConfig](https://github.com/kitUIN/ShadowObservableConfig)

`FileExt` optional: `.yaml` or `.json`

### 1. Create Configuration Class (using yaml as example)

```csharp
using ShadowObservableConfig.Attributes;
using System.Collections.ObjectModel;

[ObservableConfig(FileName = "app_config", FileExt = ".yaml", DirPath = "config", Description = "Application configuration", Version = "1.0.0")]
public partial class AppConfig
{
    [ObservableConfigProperty(Name = "AppName", Description = "Application name")]
    private string _appName = "My App";

    [ObservableConfigProperty(Name = "IsEnabled", Description = "Whether enabled")]
    private bool _isEnabled = true;

    [ObservableConfigProperty(Name = "MaxRetryCount", Description = "Maximum retry count")]
    private int _maxRetryCount = 3;

    [ObservableConfigProperty(Name = "Settings", Description = "Application settings")]
    private AppSettings _settings = new();

    [ObservableConfigProperty(Name = "Features", Description = "Feature list")]
    private ObservableCollection<string> _features = new();
}

[ObservableConfig(Description = "Application settings", Version = "1.0.0")]
public partial class AppSettings
{
    [ObservableConfigProperty(Name = "Theme", Description = "Theme")]
    private string _theme = "Light";

    [ObservableConfigProperty(Name = "Language", Description = "Language")]
    private string _language = "zh-CN";
}
```

### 2. Using in WinUI 3 (using yaml as example)

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
        Debug.WriteLine($"Configuration item '{e.FullPropertyPath}' changed: {e.OldValue} -> {e.NewValue}");
    }
}
```

### 3. XAML Data Binding

```xml
<Page x:Class="MyApp.MainPage">
    <StackPanel>
        <TextBox Header="Application name" 
                 Text="{x:Bind ViewModel.AppName, Mode=TwoWay}" />
        
        <CheckBox Content="Enable application" 
                  IsChecked="{x:Bind ViewModel.IsEnabled, Mode=TwoWay}" />
        
        <NumberBox Header="Maximum retry count" 
                   Value="{x:Bind ViewModel.MaxRetryCount, Mode=TwoWay}" />
        
        <ComboBox Header="Theme" 
                  SelectedItem="{x:Bind ViewModel.Settings.Theme, Mode=TwoWay}">
            <ComboBoxItem Content="Light" />
            <ComboBoxItem Content="Dark" />
        </ComboBox>
    </StackPanel>
</Page>
```

## 📚 Detailed Documentation

### Property Description

#### ObservableConfigAttribute
- `FileName`: Configuration file name (without extension). Not filling this field means the current class is an internal class
- `FileExt`: Configuration file extension
- `DirPath`: Configuration file directory (default is "config")
- `Description`: Configuration description
- `Version`: Configuration version

#### ObservableConfigPropertyAttribute
- `Name`: Property name in configuration file
- `Description`: Property description
- `Alias`: Property alias (only valid in yaml)
- `AutoSave`: Whether to auto-save (default is true)

### Supported Data Types

- Basic types: `string`, `int`, `double`, `bool`, `DateTime`, etc.
- Enum types: Any `enum` type
- Collection types: `ObservableCollection<T>`
- Nested objects: Other classes marked with `[ObservableConfig]`

### Auto-generated Methods

The source generator will automatically generate for each configuration class:

- Public property accessors
- `Load()` static method
- `Save()` method
- `AfterConfigInit()` partial method (can be overridden)

## 🔧 Advanced Usage

### Custom Configuration Loader

```csharp
public class CustomConfigLoader : IConfigLoader
{
    public T Load<T>(string filePath) where T : class
    {
        // Custom loading logic
        return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
    }

    public void Save<T>(T config, string filePath) where T : class
    {
        // Custom saving logic
        File.WriteAllText(filePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```

Remember to set it in `ShadowObservableConfig.GlobalSetting.Init` after customizing.

### Configuration Initialization Callback

```csharp
[ObservableConfig(FileName = "my_config")]
public partial class MyConfig
{
    [ObservableConfigProperty(Name = "Value")]
    private string _value = "default";

    partial void AfterConfigInit()
    {
        // Initialization logic after configuration loading
        Console.WriteLine($"Configuration loaded: {Value}");
    }
}
```
