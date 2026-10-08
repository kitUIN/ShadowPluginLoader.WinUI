# Resource Files

Put images and other files your plugin needs in its `Assets` folder. For example, the Emoji plugin has an image at `Assets/th.jpg`.

Include the files as content in Visual Studio, then check that they appear in the build output.

To display the image, use `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`. You'll find complete code and XAML examples in [Plugin Resource Paths](/plugin/msplugin).

For ordinary data files, use the plugin assembly's `Assembly.Location` to find the DLL directory, then build the file's actual path from there.

Finally, check your [packaging exclusions](/plugin/pack) so the files you need make it into the package.
