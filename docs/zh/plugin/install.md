# 安装、更新和删除

## 启动时加载

完成[宿主初始化](/zh/init/customloaderclass)后，从 UI 线程执行：

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

`CheckUpgradeAndRemoveAsync()` 先删除、再更新上次安排的插件，并设置处理器要求的检查标记。每次启动应在加载任何插件前等待它完成。

`ProcessAsync()` 完成预处理、元数据读取、依赖排序、程序集加载、DI 注册和插件实例化。无需再调用 `Load()`。公开的 `Load(IEnumerable<string>, IProgress<PipelineProgress>?)` 只用于处理已进入元数据缓存的插件 ID，通常由流水线的 `Outbound()` 调用。

## 输入来源

需要 `using ShadowPluginLoader.WinUI.Extensions;` 才能使用这些 `Feed` 重载。

| 输入 | 当前行为 |
| --- | --- |
| `DirectoryInfo` | 递归查找 `plugin.json`，不自动安装目录中的 `.sdow` |
| `FileInfo` 或本地 `Uri` | 读取 `plugin.json`，或解压 `.sdow` |
| HTTP / HTTPS `Uri` | 下载后按压缩包处理，不用于远程裸 JSON |
| `Type` / `Feed<TPlugin>()` | 查找程序集旁的 `{程序集名称}/plugin.json`，仍需要 DLL 和元数据文件 |
| `Type[]` / `IEnumerable<Type?>` | 逐个添加类型对应的元数据 |
| `Windows.ApplicationModel.Package` | 递归扫描包的安装目录 |
| `IMaterial` | 交给对应的预处理器 |

目录请使用 `DirectoryInfo`；当前 `Uri` 分支不能正确处理普通本地目录 URI。安装后的插件默认放在 `ApplicationData.Current.LocalFolder.Path/plugin`，下载临时文件位于 `temp`，可通过 `BaseSdkConfig` 配置。

## 安装包与进度

下面接续前面的 `loader`。依赖与插件可在同一流水线投入：

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

网络安装可将输入替换为指向插件压缩包的 HTTPS URI。流水线消费当前输入并清空列表；重复安装已加载程序集不会实现热更新。

`PipelineProgress` 包含 `Step`、`SubStep`、状态文字和百分比字段。当前实现的部分阶段使用了不一致的百分比尺度，不能直接把所有数值当作统一的 0–1 比例显示。取消令牌传给预处理和主处理阶段，不保证回滚已解压文件或已加载程序集。

捕获 `ProcessAsync()` 的异常并查看日志；部分元数据读取/解压失败会被记录并跳过，没有产出时不会报告 `Success`。处理完成后可用 `GetPlugin(id)` 核对需要的插件是否已加载。

## 更新与删除

下面两行分别演示安排更新和删除，应按用户实际操作调用：

```csharp
await loader.UpgradePlugin("ShadowExample.Plugin.Emoji",
    new Uri(@"C:\Plugins\ShadowExample.Plugin.Emoji-1.2.0.sdow"));
await loader.RemovePlugin("ShadowExample.Plugin.Hello");
```

`UpgradePlugin(string id, Uri uri)` 只接受本地 `.sdow` 文件 URI；网络包须先下载到本地。当前插件必须已加载，新版本必须更高，依赖版本必须满足要求。保留更新包直到下次启动执行完计划。

`RemovePlugin(string id)` 同样要求插件已加载。这两个方法写入计划，重启后由 `CheckUpgradeAndRemoveAsync()` 执行；不会在当前进程卸载程序集。不要用安装流水线替代已加载插件的升级。

## 查询与启用状态

```csharp
var plugins = loader.GetPlugins();
var enabledPlugins = loader.GetPlugins(isEnabled: true);
var plugin = loader.GetPlugin("ShadowExample.Plugin.Emoji");
bool? enabled = loader.IsEnabled("ShadowExample.Plugin.Emoji");
loader.DisablePlugin("ShadowExample.Plugin.Emoji");
loader.EnablePlugin("ShadowExample.Plugin.Emoji");
```

未找到插件时，`GetPlugin` 和 `IsEnabled` 返回 `null`。禁用插件不等于卸载；实例和资源仍然存在。事件的实际触发行为见[插件事件](/zh/plugin/event)。
