# Plugin Development

After you've designed your plugin loader, it's time to write plugins.
- [Create Your Plugin Project](/plugin/create)

Plugins can load the following content:
- C# code
- Custom WinUI controls (Page/UserControl)
- Custom resource dictionaries (ResourceDictionary)
- Resource files (Assets)
- i18n (Resw)

However, due to `WinUI` limitations, we need to follow some rules:
- [WinUI Controls](/plugin/control#WinUI控件规则)
- [Resource Dictionaries](/plugin/resourcedictionary#资源字典规则)
- [Resource Files](/plugin/assets#资源文件规则)

This project also provides convenient features:
- [Plugin Configuration](/plugin/config)
- [Packaging Functionality](/plugin/pack)
- [Plugin Events](/plugin/event)
