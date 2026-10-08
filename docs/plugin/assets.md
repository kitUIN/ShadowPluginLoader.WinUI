# Resource Files

Put images and other resources under the plugin's `Assets` directory and ensure they appear as WinUI content in build output and the final package. The repository Emoji plugin uses `Assets/th.jpg`.

Use `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`, resolving it with `PluginPath()` in code or `PluginImageSource` / `PluginUri` in XAML. See [Plugin Resource Paths](/plugin/msplugin) for complete examples.

Do not treat a resolved WinUI resource URI as an ordinary disk path. For arbitrary data files, follow your storage API's URI requirements, or derive the deployment directory from the plugin assembly's `Assembly.Location` and combine the actual file path.

Before distributing, check that [packaging exclusions](/plugin/pack) retain all required resources.
