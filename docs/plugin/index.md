# Plugin Development

Complete [SDK and host initialization](/init/) before [creating a plugin project](/plugin/create). Plugins can contain C# code, WinUI XAML controls, resource dictionaries, images, and localized resources.

- [Custom Controls](/plugin/control): load XAML through the extension host.
- [Resource Paths](/plugin/msplugin), [Resource Dictionaries](/plugin/resourcedictionary), and [Resource Files](/plugin/assets).
- [Plugin Configuration](/plugin/config) and [Internationalization](/advance/i18n).
- [Packaging](/plugin/pack) and [Installation, Updates, and Removal](/plugin/install).
- [Plugin Events](/plugin/event).

Plugin base classes use `AbstractPlugin<TMeta>`. Pipelines handle installation, while updates and removals execute during startup checks after restart.
