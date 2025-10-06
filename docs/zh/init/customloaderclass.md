# 创建插件加载器类

::: warning 注意

你应当先设计[插件元数据](/zh/init/metaplugin),[插件基类](/zh/init/iplugin),后再来创建加载器类

:::

## 创建插件加载器类

```csharp [ShadowExamplePluginLoader.cs]
using System;
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    [CheckAutowired]
    public partial class ShadowExamplePluginLoader :
        AbstractPluginLoader<ExampleMetaData, PluginBase>
    {
    }
}
```

- 这里使用`[CheckAutowired]`注解自动生成对应的构造函数
- (可选)可以继承修改`AbstractPluginLoader`内的部分方法


## 使用依赖注入

插件加载器加载插件的功能主要由依赖注入实现

所以我们先要创建依赖注入,必须使用`DryIoc`这个库来依赖注入

以下是一个本系统内已经创建好的[DiFactory.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/DIFactory.cs),更多请查阅`DryIoc`文档:


```csharp [DiFactory.cs]
using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Installer;
using ShadowPluginLoader.WinUI.Scanners;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Dependency injection factory
/// </summary>
public static class DiFactory
{
    /// <summary>
    /// Dependency injection container
    /// </summary>
    public static Container Services { get; }

    static DiFactory()
    {
        Services = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments));
        Services.Register(
            Made.Of(() => Serilog.Log.ForContext(Arg.Index<Type>(0)),
                r => r.ImplementationType ?? r.Parent.ImplementationType ?? typeof(object)),
            setup: Setup.With(condition: r => r.Parent.ImplementationType != null || r.ImplementationType != null));
        Services.Register<PluginEventService>(reuse: Reuse.Singleton);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Init<TAPlugin, TMeta>()
        where TAPlugin : AbstractPlugin<TMeta>
        where TMeta : AbstractPluginMetaData
    {
        MetaDataHelper.Init<TMeta>();
        var baseSdkConfig = BaseSdkConfig.Load();
        Services.RegisterInstance(baseSdkConfig);
        var innerSdkConfig = InnerSdkConfig.Load();
        Services.RegisterInstance(innerSdkConfig);
        Services.Register<IDependencyChecker<TMeta>, DependencyChecker<TMeta>>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IRemoveChecker, RemoveChecker>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IPluginScanner<TAPlugin, TMeta>, PluginScanner<TAPlugin, TMeta>>(
            serviceKey: "base", reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
            ); 
        Services.Register<IPluginInstaller<TMeta>, ZipPluginInstaller<TMeta>>(
            serviceKey: "base",  reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
                .OverrideWith(Parameters.Of.Type<IPluginScanner<TAPlugin, TMeta>>(serviceKey: "base"))
            );
    }
}
```

## 在主项目中使用

在`App.xaml.cs`文件中需要初始化一些内容

```csharp [App.xaml.cs]
public App()
{
    this.InitializeComponent();
    Init();
}

private void Init()
{
    ApplicationExtensionHost.Initialize(this);
    ShadowObservableConfig.GlobalSetting.Init(
        ApplicationData.Current.LocalFolder.Path,
        [
            new JsonConfigLoader(),
            new YamlConfigLoader()
        ]);
    DiFactory.Init<PluginBase, ExampleMetaData>();
    DiFactory.Services.Register<ShadowExamplePluginLoader>(
        reuse: Reuse.Singleton,
        made: Parameters.Of
            .Type<IDependencyChecker<ExampleMetaData>>(serviceKey: "base")
            .OverrideWith(Parameters.Of.Type<IRemoveChecker>(serviceKey: "base"))
            .OverrideWith(Parameters.Of.Type<IPluginScanner<PluginBase, ExampleMetaData>>(serviceKey: "base"))
            .OverrideWith(Parameters.Of.Type<IPluginInstaller<ExampleMetaData>>(serviceKey: "base"))
    );
}
```

## 自定义插件加载过程

有的时候我们想自定义插件加载过程,比如加载插件时需要先进行一些验证

我们可以覆写默认的加载逻辑

详见[自定义插件加载逻辑](/zh/advance/customloadplugin)