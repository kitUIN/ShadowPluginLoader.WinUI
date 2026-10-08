# 插件打包

插件写好后，把它打成一个 `.sdow` 文件，就可以交给主程序安装了。

先检查 `Tools.Config.props`：`IsPlugin` 和 `AutoPluginPackage` 应为 `true`，`IsPluginLoader` 应为 `false`。然后正常构建项目即可。

## 到哪里找安装包

构建结束后，打开项目中的 `Packages` 文件夹。默认文件名是：

- Release：`$(TargetName)-$(Version).sdow`。
- Debug：`$(TargetName)-$(Version)-Debug.sdow`。

比如 Emoji 插件的 Debug 包可以叫 `ShadowExample.Plugin.Emoji-1.1.0-Debug.sdow`。

`.sdow` 实际上是一个 ZIP 压缩包，里面放着构建输出的 DLL、插件信息和资源文件。主要目录大致如下，此外还会有 PRI 等生成文件和依赖库：

```text
ShadowExample.Plugin.Emoji.dll
ShadowExample.Plugin.Emoji/
  plugin.json
  Themes/
    ResourceDictionary1.xaml
  Assets/
    th.jpg
```

保留这个目录结构，让 `plugin.json` 放在程序集同名的子文件夹里，DLL 放在它的上一层。直接使用构建工具生成的包就不需要自己整理这些文件。

## 有些文件不想打进去怎么办

在项目根目录新建 `Plugin.Build.exclude`，每行写一个要排除的名称或通配模式：

```text [Plugin.Build.exclude]
*.pdb
hello.*
Fluent
```

这个例子会排除 `.pdb` 文件、名称符合 `hello.*` 的文件，以及名为 `Fluent` 的目录。

`*` 可以匹配任意字符，`?` 匹配一个字符。匹配时不区分大小写，只看文件或目录的名称，所以这里写 `Fluent` 即可，不要写成 `Fluent/*`。

自定义清单会替代默认清单，原来需要排除的公共依赖也要一起列进去。打包时会先从**构建输出目录**删掉匹配项，再生成压缩包；如果这份输出还要用来做别的发布，先重新构建一次。

想改输出位置或包名，可以看[自定义打包](/zh/advance/custompluginbuild)。拿到包后，继续[安装试用](/zh/plugin/install)。
