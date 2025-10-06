# 安装/更新/删除

## 安装

需要先初始化加载器,扫描插件,最后再实例化

### 初始化

- 调用`CheckUpgradeAndRemoveAsync()`

### 扫描

- `Scan(Type 插件类型)`
- `Scan<TPlugin>()`
- `Scan(IEnumerable<Type> 插件类型列表)`
- `Scan(DirectoryInfo 文件夹) `
- `Scan(FileInfo plguin.json文件)`
- `ScanAsync(Uri plugin.json本地路径)`

```csharp
var session = loader.StartScan();
await session
    .Scan(type)
    .FinishAsyn()
```

### 实例化

- 调用`Load()`

## 更新

由于WindowsAppSdk的限制,目前没有办法在运行时直接更新插件,只能重启程序后更新

- `UpgradePlugin(string id, string newVersionZip)`
  - 支持本地zip路径
  - 支持网络zip路径(自动下载)

执行该函数后,重启程序后在`CheckUpgradeAndRemoveAsync`时会自动更新插件

## 删除

由于WindowsAppSdk的限制,目前没有办法在运行时直接删除插件,只能重启程序后删除

- `RemovePlugin(string id)`

执行该函数后,重启程序后在`CheckUpgradeAndRemoveAsync`时会自动删除插件