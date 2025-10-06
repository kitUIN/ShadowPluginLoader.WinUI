# 元数据定义文件

本项目使用的`define`为`JSON Schema`的变体

在`Tool.Config.props`中启用`<IsPluginLoader>`后,会在生成时自动导出使用`[ExportMeta]`特性的类为`plugin.d.json`文件

例如:
```json
{
  "Type": "ShadowExample.Core.Plugins.ExampleMetaData",
  "Properties": {
    "Authors": {
      "Type": "System.String[]",
      "Nullable": false,
      "Required": false,
      "PropertyGroupName": "Authors"
    },
    "Url": {
      "Type": "System.String",
      "Nullable": false,
      "Required": false,
      "PropertyGroupName": "Url"
    },
    "D": {
      "Type": "System.Double",
      "Nullable": false,
      "Required": false,
      "PropertyGroupName": "D"
    },
    "F": {
      "Type": "System.Single[]",
      "Nullable": false,
      "Required": false,
      "PropertyGroupName": "F"
    },
    "Id": {
      "Type": "System.String",
      "Nullable": false,
      "Required": true,
      "PropertyGroupName": "Id"
    },
    "Name": {
      "Type": "System.String",
      "Nullable": false,
      "Required": true,
      "PropertyGroupName": "Name"
    },
    "DllName": {
      "Type": "System.String",
      "Nullable": false,
      "Required": false,
      "PropertyGroupName": "DllName"
    },
    "Version": {
      "Type": "System.String",
      "Nullable": false,
      "Required": true,
      "PropertyGroupName": "Version"
    },
    "Dependencies": {
      "Type": "System.String[]",
      "Nullable": false,
      "Required": true,
      "PropertyGroupName": "Dependencies"
    }
  }
}
```
| 名称       |      类型       | 说明 |
| ------------- | :-----------:  | ---- |
| `Type`      |   `string`    |  元数据类名,[支持的元数据项类型](/zh/advance/meta#支持的元数据项类型) |
| `Properties` |   `Object`     |  元数据项,具体可配置项可查阅[`[Meta]`特性](/zh/init/metaplugin.html#元数据项的额外配置) |

