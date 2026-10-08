# 资源文件

插件里要用到图片或其他文件时，可以把它们放进项目的 `Assets` 文件夹。比如 Emoji 插件的图片放在 `Assets/th.jpg`。

在 Visual Studio 中把这些文件作为内容包含进项目，构建后检查一下输出目录里有没有它们。

显示图片时，可以使用 `ms-plugin://ShadowExample.Plugin.Emoji/Assets/th.jpg`。代码和 XAML 的完整写法都在[插件资源路径](/zh/plugin/msplugin)一节。

如果要读取普通数据文件，可以从插件程序集的 `Assembly.Location` 找到 DLL 所在目录，再拼出文件的实际路径。

最后别忘了检查[打包排除清单](/zh/plugin/pack)，确保需要的文件都在插件包里。
