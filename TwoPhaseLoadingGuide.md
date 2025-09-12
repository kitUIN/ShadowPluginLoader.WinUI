# 两阶段插件加载指南

## 概述

两阶段插件加载是一种优化插件系统启动性能的技术，将插件加载过程分为两个阶段：

1. **DLL预加载阶段**：只加载程序集，不实例化插件
2. **延迟实例化阶段**：在主程序启动后按需或批量实例化插件

## 优势

- **快速启动**：主程序可以快速启动，DLL加载时间被最小化
- **按需加载**：可以根据需要只实例化特定的插件
- **更好的用户体验**：用户界面可以更快地显示
- **资源优化**：避免不必要的插件实例化

## 使用方法

### 基本用法

```csharp
// 创建插件加载器
var loader = new ShadowExamplePluginLoader(logger, pluginEventService);

// 阶段1：扫描插件
loader.Scan(typeof(EmojiPlugin));
loader.Scan(typeof(HelloPlugin));

// 阶段2：预加载DLL（快速启动）
await loader.CheckUpgradeAndRemoveAsync();
var preloadedPlugins = await loader.PreloadAsync();

// 阶段3：延迟实例化（在主程序启动后）
var instantiatedCount = await loader.InstantiatePluginsAsync();
```

### 按需实例化

```csharp
// 只实例化特定的插件
var instantiatedCount = await loader.InstantiatePluginsAsync(new[] { "Emoji" });

// 获取预加载的插件信息
var preloadedPlugin = loader.GetPreloadedPlugin("Emoji");
if (preloadedPlugin != null)
{
    Console.WriteLine($"插件 {preloadedPlugin.MetaData.Name} 已预加载但未实例化");
}
```

### 批量延迟实例化

```csharp
// 在主程序完全启动后批量实例化所有插件
var instantiatedCount = await loader.InstantiatePluginsAsync();
Console.WriteLine($"批量实例化了 {instantiatedCount} 个插件");
```

## API 参考

### 新增方法

#### `PreloadAsync()`
预加载插件DLL，不进行实例化。

**返回值**：`Task<List<PreloadedPluginData<TMeta>>>`

**示例**：
```csharp
var preloadedPlugins = await loader.PreloadAsync();
```

#### `InstantiatePluginsAsync(IEnumerable<string>? pluginIds = null)`
实例化预加载的插件。

**参数**：
- `pluginIds`：要实例化的插件ID列表，为空则实例化所有

**返回值**：`Task<int>` - 实例化的插件数量

**示例**：
```csharp
// 实例化所有插件
var count = await loader.InstantiatePluginsAsync();

// 只实例化指定插件
var count = await loader.InstantiatePluginsAsync(new[] { "Emoji", "Hello" });
```

#### `GetPreloadedPlugins()`
获取所有预加载的插件数据。

**返回值**：`IList<PreloadedPluginData<TMeta>>`

#### `GetPreloadedPlugin(string id)`
获取指定插件的预加载数据。

**参数**：
- `id`：插件ID

**返回值**：`PreloadedPluginData<TMeta>?`

### PreloadedPluginData 类

预加载插件的数据结构：

```csharp
public class PreloadedPluginData<TMeta>
{
    public TMeta MetaData { get; init; }           // 插件元数据
    public Type MainPluginType { get; init; }      // 插件主类型
    public Assembly Assembly { get; init; }        // 插件程序集
    public string InstallerId { get; init; }       // 安装器ID
    public string Path { get; init; }              // 插件路径
    public bool IsInstantiated { get; set; }       // 是否已实例化
    public DateTime? InstantiatedAt { get; set; }  // 实例化时间
    public AbstractPlugin<TMeta>? Instance { get; set; } // 插件实例
    public List<PluginDependency> Dependencies { get; init; } // 依赖关系
    public int Priority { get; init; }             // 优先级
    public Version Version { get; init; }          // 版本
}
```

## 性能对比

### 传统加载方式
```
扫描 → 安装 → 加载DLL → 实例化插件
总时间 = 扫描时间 + 安装时间 + DLL加载时间 + 实例化时间
```

### 两阶段加载方式
```
阶段1（快速启动）：扫描 → 安装 → 加载DLL
阶段2（延迟）：实例化插件

启动时间 = 扫描时间 + 安装时间 + DLL加载时间
总时间 = 启动时间 + 实例化时间
```

### 性能提升
- **启动时间减少**：通常可减少 30-70% 的启动时间
- **内存优化**：避免不必要的插件实例化
- **用户体验**：界面可以更快显示

## 最佳实践

### 1. 启动阶段
```csharp
// 在应用启动时只进行预加载
public async Task OnApplicationStartup()
{
    var loader = GetPluginLoader();
    
    // 扫描插件
    ScanPlugins(loader);
    
    // 预加载DLL
    await loader.CheckUpgradeAndRemoveAsync();
    await loader.PreloadAsync();
    
    // 主程序UI可以立即显示
    ShowMainUI();
}
```

### 2. 延迟实例化
```csharp
// 在UI显示后或用户交互时进行实例化
public async Task OnUIReady()
{
    var loader = GetPluginLoader();
    
    // 批量实例化所有插件
    await loader.InstantiatePluginsAsync();
    
    // 或者按需实例化
    await loader.InstantiatePluginsAsync(new[] { "EssentialPlugin" });
}
```

### 3. 错误处理
```csharp
try
{
    var preloadedPlugins = await loader.PreloadAsync();
    
    foreach (var plugin in preloadedPlugins)
    {
        try
        {
            await loader.InstantiatePluginsAsync(new[] { plugin.MetaData.Id });
        }
        catch (Exception ex)
        {
            logger.Warning("Failed to instantiate plugin {Id}: {Error}", 
                plugin.MetaData.Id, ex.Message);
        }
    }
}
catch (Exception ex)
{
    logger.Error("Failed to preload plugins: {Error}", ex.Message);
}
```

## 注意事项

1. **内存使用**：预加载阶段会占用更多内存，因为DLL已加载但未实例化
2. **依赖关系**：确保在实例化时依赖关系仍然有效
3. **错误处理**：预加载阶段的错误不会影响主程序启动
4. **状态管理**：需要正确管理插件的预加载和实例化状态

## 迁移指南

### 从传统加载迁移到两阶段加载

**传统方式**：
```csharp
loader.Scan(pluginType);
await loader.CheckUpgradeAndRemoveAsync();
await loader.LoadAsync(); // 一次性完成所有操作
```

**两阶段方式**：
```csharp
loader.Scan(pluginType);
await loader.CheckUpgradeAndRemoveAsync();
await loader.PreloadAsync(); // 只预加载DLL
// ... 主程序启动 ...
await loader.InstantiatePluginsAsync(); // 延迟实例化
```

### 兼容性
- 原有的 `LoadAsync()` 方法仍然可用
- 新的两阶段方法是对现有API的扩展
- 可以逐步迁移现有代码