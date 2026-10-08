# Tools.Config.props

工具在首次构建时将默认 `Tools.Config.props` 复制到项目根目录。也可以预先手动创建，再按项目角色修改。文件名中的 `Tools` 为复数。

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

| 属性 | 默认值 | 行为 |
| --- | --- | --- |
| `IsPluginLoader` | `false` | 导出 `[ExportMeta]` 的 Schema，并打入 SDK NuGet 包 |
| `IsPlugin` | `false` | 导入插件构建目标，复制 `plugin.d.json`，生成元数据并打包 |
| `AutoPluginPackage` | `true` | 插件构建时执行打包目标 |
| `PluginMisxPackage` | `false` | 额外调用 MSIX 打包脚本，需证书配置；拼写以此为准 |
| `AutoGenerateI18N` | `true` | 将 `Strings/**/*` 作为源生成器的附加输入 |
| `DebugSourceGenerator` | `false` | 插件项目将生成源码输出到 `GeneratedFiles` |

SDK 设置 `IsPluginLoader=true`、`IsPlugin=false`；插件反过来；普通宿主两者都为 false。不要同时开启两者，当前目标导入在这种情况下优先采用 SDK 分支。

源 `plugin.json` 必须自行编写，工具会渲染、校验并在输出目录生成最终文件。当前 `ReadMetaData` 挂在 `PackagePlugin` 前执行；关闭自动打包时，自定义流程应显式保留元数据生成步骤，并核对输出中的 JSON 是否更新。

`DebugSourceGenerator` 生成的文件用于检查，不要手动编辑或重复加入编译。构造函数生成由 `[Autowired]` / `[CheckAutowired]` 触发，与 `AutoGenerateI18N` 无关。

参见[自定义打包](/zh/advance/custompluginbuild)、[依赖注入](/zh/advance/quickdi)和[国际化](/zh/advance/i18n)。
