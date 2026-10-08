# 自定义插件打包

默认打包方式已经够用。如果你想换一个输出文件夹、改包名，或者接入自己的构建流程，可以在这里调整。

## 修改输出位置和包名

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

`PluginPackagePath` 是输出目录，`PluginPackageName` 是文件名，`PluginPackageExt` 是扩展名。比如把 `PluginPackagePath` 改成另一个目录，下次构建的包就会放到那里。

Debug 构建还会在文件名后加上 `-Debug`。扩展名建议保留 `.sdow`，这样可以直接使用加载器的安装和更新方法。

这个文件会替代工具自带的默认配置，可以从上面的完整示例开始改。

## 调整排除清单

把不需要的文件写进项目里的 `Plugin.Build.exclude`，具体格式见[插件打包](/zh/plugin/pack)。项目中没有这个文件时，工具会使用自带的默认清单。

自己写清单时，也要保留需要排除的公共依赖，因为两份清单不会自动合并。

## 接入自己的构建流程

需要进一步定制时，可以替换对应的 MSBuild 文件：

| 属性 / 本地文件 | 负责的步骤 |
| --- | --- |
| `ToolTargetsFile` / `Tool.targets` | 复制 `Tools.Config.props` 等通用操作 |
| `LoaderToolTargetsFile` / `Loader.Build.targets` | 导出 SDK 元数据、生成 NuGet 包 |
| `PluginToolTargetsFile` / `Plugin.Build.targets` | 复制定义文件、生成插件信息、打包插件 |
| `LoaderPropsFile` / `Loaders.Build.props` | 告诉插件项目去哪里找 `PluginDFile` |
| `ExcludeFile` | 指定排除清单，多个文件用分号分隔 |

替换前可以先复制工具自带的文件，再修改需要的步骤。记得保留元数据生成和资源复制，否则包里可能缺少加载时要用的文件。

## 生成 MSIX 包

如果需要 MSIX，把 `PluginMisxPackage` 设为 `true`，再填写证书路径 `AppCertPath` 和密码 `AppCertPassword`。构建时会通过 `PackageMsix.ps1` 额外打包。

安装 `.sdow` 仍使用前面的流水线。MSIX 安装到 Windows 后，可以通过 `Feed(Package)` 扫描包里的插件。
