# 插件开发

先完成[SDK 与宿主初始化](/zh/init/)，再[创建插件项目](/zh/plugin/create)。插件可以包含 C# 代码、WinUI XAML 控件、资源字典、图片和本地化资源。

- [自定义控件](/zh/plugin/control)：使用扩展宿主加载 XAML。
- [资源路径](/zh/plugin/msplugin)、[资源字典](/zh/plugin/resourcedictionary)与[资源文件](/zh/plugin/assets)。
- [插件配置](/zh/plugin/config)与[国际化](/zh/advance/i18n)。
- [打包](/zh/plugin/pack)和[安装、更新、删除](/zh/plugin/install)。
- [插件事件](/zh/plugin/event)。

插件基类使用 `AbstractPlugin<TMeta>`。安装由流水线完成，更新和删除则在重启后的启动检查阶段执行。
