# 资源文件

把图片等资源放入插件项目的 `Assets` 目录，并确保其作为 WinUI 内容出现在构建输出和最终包中。例如仓库 Emoji 插件的 `Assets/th.jpg`。

资源路径使用 `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`，在代码中调用 `PluginPath()`，或在 XAML 中使用 `PluginImageSource` / `PluginUri`。完整示例见[插件资源路径](/zh/plugin/msplugin)。

不要把解析后的 WinUI 资源 URI 当作普通磁盘路径。读取任意数据文件时，按所使用存储 API 的要求处理 URI，或从插件程序集的 `Assembly.Location` 推导部署目录并组合实际文件路径。

发布前检查[打包排除清单](/zh/plugin/pack)没有移除需要的资源。
