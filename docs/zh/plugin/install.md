# 安装、更新和删除

有了加载器和插件，接下来就可以在主程序里把插件加载进来了。

## 启动时加载插件

完成[主程序初始化](/zh/init/customloaderclass)后，在窗口的 UI 线程中调用：

```csharp
using DryIoc;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Extensions;
using System.IO;

var loader = DiFactory.Services.Resolve<ShadowExamplePluginLoader>();
await loader.CheckUpgradeAndRemoveAsync();
await loader.CreatePipeline()
    .Feed(new DirectoryInfo(loader.PluginFolderPath))
    .ProcessAsync();
```

先调用 `CheckUpgradeAndRemoveAsync()`，处理上次退出前安排的删除和更新。等它完成后，再加载插件。

后面三步可以这样理解：`CreatePipeline()` 创建一次加载任务，`Feed(...)` 告诉它去哪里找插件，`ProcessAsync()` 开始处理。加载器会读插件信息、检查依赖，再创建插件实例，不用额外调用 `Load()`。

## 可以从哪里加载

加上 `using ShadowPluginLoader.WinUI.Extensions;` 后，`Feed` 可以接收这些参数：

| 参数 | 用法 |
| --- | --- |
| `DirectoryInfo` | 在目录和子目录中查找 `plugin.json` |
| `FileInfo` 或本地 `Uri` | 指向一个 `plugin.json` 或 `.sdow` 包 |
| HTTP / HTTPS `Uri` | 下载并安装插件压缩包 |
| `Type` / `Feed<TPlugin>()` | 加载该类型对应的插件，需要程序集旁有 `{程序集名称}/plugin.json` |
| `Type[]` / `IEnumerable<Type?>` | 一次传入多个插件类型 |
| `Windows.ApplicationModel.Package` | 从已安装的 Windows 包中查找插件 |
| `IMaterial` | 使用[自定义处理方式](/zh/advance/customloadplugin) |

扫描文件夹时用 `DirectoryInfo`。它会查找已经解压好的插件；如果文件夹里放的是 `.sdow`，需要把包的路径传给 `Feed`。

插件默认安装在应用本地数据目录的 `plugin` 文件夹中，下载的临时文件放在 `temp`。需要换位置时，可以修改 `BaseSdkConfig`。

## 安装插件包

继续使用上面的 `loader`，把包的路径传进去即可。下面把 Hello 和 Emoji 一起安装，并输出处理进度：

```csharp
using System;
using System.Diagnostics;
using System.Threading;
using ShadowPluginLoader.WinUI.Models;

var progress = new Progress<PipelineProgress>(p =>
    Debug.WriteLine($"{p.Step}: {p.SubStep} {p.TotalStatusValue}"));
using var cancellation = new CancellationTokenSource();

await loader.CreatePipeline()
    .Feed(new Uri(@"C:\Plugins\ShadowExample.Plugin.Hello-1.1.6.sdow"))
    .Feed(new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.1.0.sdow"))
    .ProcessAsync(progress, cancellation.Token);
```

从网络安装时，把路径换成插件压缩包的 HTTPS 地址即可。如果要取消，可以调用 `cancellation.Cancel()`；已经解压或加载的内容不会因此自动撤回。

示例使用 `Step`、`SubStep` 和状态文字显示进度。百分比字段在不同阶段的计算方式还不统一，暂时建议按步骤显示状态。

安装失败时可以捕获异常并查看日志。处理完成后，调用 `GetPlugin(id)` 就能确认插件有没有加载成功。

## 更新和删除

更新和删除都需要重启程序才会生效。下面分别演示这两个操作：

```csharp
await loader.UpgradePlugin("ShadowExample.Plugin.Emoji",
    new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.2.0.sdow"));
await loader.RemovePlugin("ShadowExample.Plugin.Hello");
```

更新时传入本地 `.sdow` 路径；网络上的包要先下载下来。要更新的插件需要已经加载，新版本要高于旧版本，依赖也要满足要求。更新包先别删，下次启动还会用到它。

删除同样是针对已加载的插件。调用后，下次启动时的 `CheckUpgradeAndRemoveAsync()` 会执行这些操作。

## 查看和切换启用状态

```csharp
var plugins = loader.GetPlugins();
var enabledPlugins = loader.GetPlugins(isEnabled: true);
var plugin = loader.GetPlugin("ShadowExample.Plugin.Emoji");
bool? enabled = loader.IsEnabled("ShadowExample.Plugin.Emoji");
loader.DisablePlugin("ShadowExample.Plugin.Emoji");
loader.EnablePlugin("ShadowExample.Plugin.Emoji");
```

插件不存在时，`GetPlugin` 和 `IsEnabled` 会返回 `null`。禁用后插件仍留在内存中，所以插件要在 `Disabled()` 里停止自己的工作。

如果想在界面上显示加载、启用等状态变化，可以继续看[插件事件](/zh/plugin/event)。
