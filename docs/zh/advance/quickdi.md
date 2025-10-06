# 快速依赖注入

```csharp
public EmojiPlugin(ILogger logger, PluginEventService pluginEventService) : base(logger, pluginEventService)
{

}
```


如果你觉得像上文一样编写依赖注入的构造函数过于麻烦

这里提供两个特性快速开发

## [Autowired]

该特性针对属性

```csharp
[Autowired]
public PluginLoader PluginService { get;}
[Autowired]
public ICallableService Caller { get;}
[Autowired]
public INavigateService NavigateService { get;}
```
将会自动生成构造函数
```csharp
// Automatic Generate From ShadowPluginLoader.SourceGenerator

namespace ShadowViewer.ViewModels;

public partial class TitleBarViewModel
{
    public TitleBarViewModel(global::ShadowViewer.Core.PluginLoader pluginService, global::ShadowViewer.Core.Services.ICallableService caller, global::ShadowViewer.Core.Services.INavigateService navigateService)
    {
       PluginService = pluginService;
       Caller = caller;
       NavigateService = navigateService;
    }
}
```

## [CheckAutowired]

该特性针对类

将自动检测当前类是否需要依赖注入构造函数, 如果需要则直接进行构建
```csharp
[CheckAutowired]
public partial class TitleBarViewModel
{
    
}
```
将会自动生成构造函数
```csharp
// Automatic Generate From ShadowPluginLoader.SourceGenerator

namespace ShadowViewer.ViewModels;

public partial class TitleBarViewModel
{
    public TitleBarViewModel(global::ShadowViewer.Core.PluginLoader pluginService, global::ShadowViewer.Core.Services.ICallableService caller, global::ShadowViewer.Core.Services.INavigateService navigateService)
    {
       PluginService = pluginService;
       Caller = caller;
       NavigateService = navigateService;
    }
}
```
