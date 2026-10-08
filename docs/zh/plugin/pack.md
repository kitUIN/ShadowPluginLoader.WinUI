# 插件打包

在 `Tools.Config.props` 中设置 `IsPlugin=true`、`IsPluginLoader=false` 和 `AutoPluginPackage=true`，然后构建插件。

## 输出

默认输出到 `$(ProjectDir)Packages/`：

- Release：`$(TargetName)-$(Version).sdow`。
- Debug：`$(TargetName)-$(Version)-Debug.sdow`。

`.sdow` 是 ZIP 格式，打包来源为插件构建输出目录。它包含程序集、生成的元数据、XAML 和资源等。下面仅示意关键结构，实际部署还需保留生成的 PRI 和必要依赖：

```text
ShadowExample.Plugin.Emoji.dll
ShadowExample.Plugin.Emoji/
  plugin.json
  Themes/
    ResourceDictionary1.xaml
  Assets/
    th.jpg
```

`plugin.json` 位于以程序集命名的子目录，DLL 位于其上一级。不要只把源模板或 DLL 放入包中；预处理器查找的是以 `/plugin.json` 结尾的 ZIP 条目。

## 排除文件

在项目目录创建 `Plugin.Build.exclude`，每行一个模式：

```text [Plugin.Build.exclude]
*.pdb
hello.*
Fluent
```

当前打包器递归枚举输出目录，以**文件或目录名称**进行不区分大小写的匹配；`*` 匹配任意字符，`?` 匹配单个字符。`Fluent` 匹配任意层级的同名目录；带路径的 `Fluent/*` 或 `core/**/text.txt` 不匹配，因为比较的不是相对路径。

自定义清单替代工具默认清单。排除操作会实际删除构建输出目录中的匹配文件/目录，然后再压缩；不会修改源文件。若输出还要用于其他发布方式，请先重新构建。

输出命名、目标替换和可选 MSIX 见[自定义打包](/zh/advance/custompluginbuild)。安装生成的包见[安装与管理](/zh/plugin/install)。
