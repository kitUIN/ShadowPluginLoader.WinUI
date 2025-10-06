# 插件开发

在你设计完你的插件加载器后,是该编写插件了
- [创建你的插件项目](/zh/plugin/create)


插件可以加载以下内容:
- C#代码
- 自定义WinUI控件(Page/UserControl)
- 自定义资源字典(ResourceDictionary)
- 资源文件(Assets)
- i18n(Resw)

但由于`WinUI`的限制,我们需要遵循一些规则:
- [WinUI控件](/zh/plugin/control#WinUI控件规则)
- [资源字典](/zh/plugin/resourcedictionary#资源字典规则)
- [资源文件](/zh/plugin/assets#资源文件规则)



本项目还提供了便利的
- [插件设置项](/zh/plugin/settings)
- [打包功能](/zh/plugin/pack)
- [插件事件](/zh/plugin/event)