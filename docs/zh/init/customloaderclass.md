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

以下是一个本系统内已经创建好的[DiFactory.cs](https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/DIFactory.cs),更多请查阅`DryIoc`文档。

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
    DiFactory.RegisterPluginLoader<PluginLoader>();
}
```

## 自定义插件加载过程

有的时候我们想自定义插件加载过程,比如加载插件时需要先进行一些验证

我们可以覆写默认的加载逻辑

详见[自定义插件加载逻辑](/zh/advance/customloadplugin)