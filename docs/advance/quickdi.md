# Quick Dependency Injection

```csharp
public EmojiPlugin(ILogger logger, PluginEventService pluginEventService) : base(logger, pluginEventService)
{

}
```

If you find writing dependency injection constructors like the above too cumbersome,

Here are two attributes for quick development:

## [Autowired]

This attribute targets properties:

```csharp
[Autowired]
public PluginLoader PluginService { get;}
[Autowired]
public ICallableService Caller { get;}
[Autowired]
public INavigateService NavigateService { get;}
```

Will automatically generate constructor:
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

This attribute targets classes:

Will automatically detect if the current class needs a dependency injection constructor, and build it directly if needed:
```csharp
[CheckAutowired]
public partial class TitleBarViewModel
{
    
}
```

Will automatically generate constructor:
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
