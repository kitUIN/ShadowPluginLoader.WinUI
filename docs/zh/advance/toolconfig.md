# Tools.Config.props

这个文件用来告诉构建工具：项目是 SDK 还是插件，要不要自动打包，要不要生成多语言帮助类。

第一次构建时，工具会把默认文件复制到项目根目录。你也可以提前创建它：

```xml [Tools.Config.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <IsPluginLoader>false</IsPluginLoader>
    <IsPlugin>false</IsPlugin>
    <AutoPluginPackage>true</AutoPluginPackage>
    <PluginMisxPackage>false</PluginMisxPackage>
    <AutoGenerateI18N>true</AutoGenerateI18N>
    <DebugSourceGenerator>false</DebugSourceGenerator>
  </PropertyGroup>
</Project>
```

## 先选好项目类型

- SDK 项目：`IsPluginLoader=true`，`IsPlugin=false`。
- 插件项目：`IsPluginLoader=false`，`IsPlugin=true`。
- 普通主程序：两项都设为 `false`。

`IsPluginLoader` 和 `IsPlugin` 选一个开启就好。

## 其他开关

| 属性 | 默认值 | 用来做什么 |
| --- | --- | --- |
| `AutoPluginPackage` | `true` | 构建插件时自动生成安装包 |
| `PluginMisxPackage` | `false` | 额外生成 MSIX 包，需要配置证书 |
| `AutoGenerateI18N` | `true` | 从 `Strings` 中的资源文件生成多语言帮助类 |
| `DebugSourceGenerator` | `false` | 把插件项目的生成代码保存到 `GeneratedFiles`，方便查看 |

`plugin.json` 模板仍需要自己填写，构建工具负责替换变量并生成最终文件。如果关闭自动打包，也要在自己的构建流程中安排元数据生成，具体见[自定义打包](/zh/advance/custompluginbuild)。

想看工具生成了什么代码，可以打开 `DebugSourceGenerator`。这些文件仅用于查看，修改自己的源文件后重新构建即可。

多语言和构造函数的用法分别见[国际化](/zh/advance/i18n)与[快速依赖注入](/zh/advance/quickdi)。
