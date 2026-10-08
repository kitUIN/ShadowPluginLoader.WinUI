# 自定义插件打包

## 输出位置与名称

在插件项目根目录创建 `Plugin.Build.props`：

```xml [Plugin.Build.props]
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PluginPackagePath>$(ProjectDir)Packages/</PluginPackagePath>
    <PluginPackageName>$(TargetName)-$(Version)</PluginPackageName>
    <PluginPackageExt>.sdow</PluginPackageExt>
  </PropertyGroup>
</Project>
```

此文件会替代工具包默认的同名文件，因此应保留需要的默认项。`PluginPackagePath` 决定目录，`PluginPackageName` 决定不含扩展名的文件名，`PluginPackageExt` 决定扩展名。Debug 构建会另外追加 `-Debug`。

保留 `.sdow` 可直接使用默认 `Feed(Uri)` 与升级接口。仅改变扩展名不会改变压缩格式；其他后缀需要自定义输入处理，升级接口仍只接受 `.sdow`。

## 排除文件

默认使用项目内的 `Plugin.Build.exclude`，若不存在则使用工具包内置清单。自定义清单是替代关系，应一并保留仍需排除的宿主公共程序集。详见[插件打包](/zh/plugin/pack)。

## 替换 MSBuild 目标

| 属性 / 本地文件 | 默认用途 |
| --- | --- |
| `ToolTargetsFile` / `Tool.targets` | 通用构建目标，默认复制 `Tools.Config.props` |
| `LoaderToolTargetsFile` / `Loader.Build.targets` | SDK 元数据导出和 NuGet 打包 |
| `PluginToolTargetsFile` / `Plugin.Build.targets` | 插件定义复制、模板生成和打包 |
| `LoaderPropsFile` / `Loaders.Build.props` | SDK 包向插件传递 `PluginDFile` |
| `ExcludeFile` | 排除清单路径，工具支持分号分隔多个文件 |

替换目标文件将接管整套对应流程，应保留必要的元数据生成和资源复制步骤。当前 `DefaultExclude` 属性虽在默认 props 中声明，但打包代码没有按它启停内置规则；用实际排除清单控制内容。

## 可选 MSIX

工具使用拼写为 `PluginMisxPackage` 的开关，并读取 `AppCertPath`、`AppCertPassword`。启用时在普通打包后调用 `PackageMsix.ps1`，需要可用的证书和打包环境。它不代表默认安装流水线可读取 `.msix`；`Feed(Package)` 接收已安装的 Windows 包。
