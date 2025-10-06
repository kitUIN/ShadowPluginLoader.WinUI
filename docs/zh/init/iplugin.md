# 创建插件基类

:::  warning 注意

你应当先设计[插件元数据](/zh/init/metaplugin),再来设置插件基类

:::


```csharp [PluginBase.cs]
// 示例代码
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

public abstract class PluginBase: AbstractPlugin
{
    public abstract string GetEmoji();
}
```

- 插件基类**必须**继承`AbstractPlugin`
- (可选)`AbstractPlugin`中默认设置了一些函数,你可以在此基础上增改
