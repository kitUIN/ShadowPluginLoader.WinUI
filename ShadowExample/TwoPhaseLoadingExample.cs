using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowExample;

/// <summary>
/// 两阶段加载示例
/// 展示如何将插件加载分为DLL预加载和延迟实例化两个阶段
/// </summary>
public class TwoPhaseLoadingExample
{
    private readonly ShadowExamplePluginLoader _loader;
    private readonly ILogger _logger;

    public TwoPhaseLoadingExample(ShadowExamplePluginLoader loader, ILogger logger)
    {
        _loader = loader;
        _logger = logger;
    }

    /// <summary>
    /// 示例1：快速启动模式 - 只预加载DLL，延迟实例化
    /// </summary>
    public async Task QuickStartExample()
    {
        _logger.Information("=== 快速启动模式示例 ===");
        
        var stopwatch = Stopwatch.StartNew();
        
        // 阶段1：扫描插件
        _loader.Scan(typeof(ShadowExample.Plugin.Emoji.EmojiPlugin));
        _loader.Scan(typeof(ShadowExample.Plugin.Hello.HelloPlugin));
        
        // 阶段2：预加载DLL（不实例化）
        await _loader.CheckUpgradeAndRemoveAsync();
        var preloadedPlugins = await _loader.PreloadAsync();
        
        stopwatch.Stop();
        _logger.Information("DLL预加载完成，耗时: {ms}ms，预加载插件数量: {count}", 
            stopwatch.ElapsedMilliseconds, preloadedPlugins.Count);
        
        // 显示预加载的插件信息
        foreach (var plugin in preloadedPlugins)
        {
            _logger.Information("预加载插件: {id} v{version} - {name}", 
                plugin.MetaData.Id, plugin.MetaData.Version, plugin.MetaData.Name);
        }
        
        // 阶段3：延迟实例化（可以在主程序启动后调用）
        _logger.Information("主程序启动完成，开始延迟实例化插件...");
        await InstantiatePluginsOnDemand();
    }

    /// <summary>
    /// 示例2：按需实例化 - 只实例化需要的插件
    /// </summary>
    public async Task OnDemandInstantiationExample()
    {
        _logger.Information("=== 按需实例化示例 ===");
        
        // 只实例化Emoji插件
        var instantiatedCount = await _loader.InstantiatePluginsAsync(new[] { "Emoji" });
        _logger.Information("按需实例化了 {count} 个插件", instantiatedCount);
        
        // 获取已实例化的插件
        var emojiPlugin = _loader.GetPlugin("Emoji");
        if (emojiPlugin != null)
        {
            _logger.Information("Emoji插件已实例化: {displayName}", emojiPlugin.DisplayName);
        }
    }

    /// <summary>
    /// 示例3：批量延迟实例化 - 在主程序完全启动后批量实例化
    /// </summary>
    public async Task BatchDelayedInstantiationExample()
    {
        _logger.Information("=== 批量延迟实例化示例 ===");
        
        var stopwatch = Stopwatch.StartNew();
        
        // 批量实例化所有预加载的插件
        var instantiatedCount = await _loader.InstantiatePluginsAsync();
        
        stopwatch.Stop();
        _logger.Information("批量实例化完成，耗时: {ms}ms，实例化插件数量: {count}", 
            stopwatch.ElapsedMilliseconds, instantiatedCount);
        
        // 显示所有已实例化的插件
        var allPlugins = _loader.GetPlugins();
        foreach (var plugin in allPlugins)
        {
            _logger.Information("已实例化插件: {id} - {displayName} (启用状态: {enabled})", 
                plugin.Id, plugin.DisplayName, plugin.IsEnabled);
        }
    }

    /// <summary>
    /// 示例4：性能对比 - 传统加载 vs 两阶段加载
    /// </summary>
    public async Task PerformanceComparisonExample()
    {
        _logger.Information("=== 性能对比示例 ===");
        
        // 传统加载方式
        var traditionalLoader = new ShadowExamplePluginLoader(_logger, new PluginEventService(_logger));
        traditionalLoader.Scan(typeof(ShadowExample.Plugin.Emoji.EmojiPlugin));
        traditionalLoader.Scan(typeof(ShadowExample.Plugin.Hello.HelloPlugin));
        
        var traditionalStopwatch = Stopwatch.StartNew();
        await traditionalLoader.CheckUpgradeAndRemoveAsync();
        await traditionalLoader.LoadAsync();
        traditionalStopwatch.Stop();
        
        _logger.Information("传统加载方式耗时: {ms}ms", traditionalStopwatch.ElapsedMilliseconds);
        
        // 两阶段加载方式
        var twoPhaseLoader = new ShadowExamplePluginLoader(_logger, new PluginEventService(_logger));
        twoPhaseLoader.Scan(typeof(ShadowExample.Plugin.Emoji.EmojiPlugin));
        twoPhaseLoader.Scan(typeof(ShadowExample.Plugin.Hello.HelloPlugin));
        
        var twoPhaseStopwatch = Stopwatch.StartNew();
        await twoPhaseLoader.CheckUpgradeAndRemoveAsync();
        
        // 阶段1：预加载
        var preloadStopwatch = Stopwatch.StartNew();
        await twoPhaseLoader.PreloadAsync();
        preloadStopwatch.Stop();
        
        // 阶段2：延迟实例化
        var instantiateStopwatch = Stopwatch.StartNew();
        await twoPhaseLoader.InstantiatePluginsAsync();
        instantiateStopwatch.Stop();
        
        twoPhaseStopwatch.Stop();
        
        _logger.Information("两阶段加载总耗时: {total}ms", twoPhaseStopwatch.ElapsedMilliseconds);
        _logger.Information("  - 预加载阶段: {preload}ms", preloadStopwatch.ElapsedMilliseconds);
        _logger.Information("  - 实例化阶段: {instantiate}ms", instantiateStopwatch.ElapsedMilliseconds);
        
        // 计算性能提升
        var improvement = ((double)(traditionalStopwatch.ElapsedMilliseconds - preloadStopwatch.ElapsedMilliseconds) 
                          / traditionalStopwatch.ElapsedMilliseconds) * 100;
        _logger.Information("启动性能提升: {improvement:F1}%", improvement);
    }

    /// <summary>
    /// 示例5：插件状态管理
    /// </summary>
    public async Task PluginStateManagementExample()
    {
        _logger.Information("=== 插件状态管理示例 ===");
        
        // 获取预加载的插件状态
        var preloadedPlugins = _loader.GetPreloadedPlugins();
        foreach (var plugin in preloadedPlugins)
        {
            _logger.Information("预加载插件状态: {id} - 已实例化: {instantiated}, 实例化时间: {time}", 
                plugin.MetaData.Id, plugin.IsInstantiated, plugin.InstantiatedAt);
        }
        
        // 检查特定插件的状态
        var emojiPreloaded = _loader.GetPreloadedPlugin("Emoji");
        if (emojiPreloaded != null)
        {
            _logger.Information("Emoji插件预加载状态: 程序集={assembly}, 类型={type}", 
                emojiPreloaded.Assembly.FullName, emojiPreloaded.MainPluginType.Name);
        }
    }

    /// <summary>
    /// 延迟实例化插件的示例方法
    /// 这个方法可以在主程序启动后的任何时间调用
    /// </summary>
    private async Task InstantiatePluginsOnDemand()
    {
        _logger.Information("开始延迟实例化插件...");
        
        var stopwatch = Stopwatch.StartNew();
        var instantiatedCount = await _loader.InstantiatePluginsAsync();
        stopwatch.Stop();
        
        _logger.Information("延迟实例化完成，耗时: {ms}ms，实例化插件数量: {count}", 
            stopwatch.ElapsedMilliseconds, instantiatedCount);
    }
}