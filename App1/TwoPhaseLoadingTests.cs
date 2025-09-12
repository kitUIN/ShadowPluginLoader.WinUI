using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Serilog;
using ShadowExample.Core;
using ShadowExample.Plugin.Emoji;
using ShadowPluginLoader.WinUI.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace App1;

[TestClass]
public class TwoPhaseLoadingTests
{
    private ShadowExamplePluginLoader loader;
    private ILogger logger;

    [TestInitialize]
    public void Setup()
    {
        logger = Log.ForContext<TwoPhaseLoadingTests>();
        loader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
    }

    [TestMethod("两阶段加载 - 预加载阶段")]
    public async Task PreloadPhase_ShouldLoadDllsWithoutInstantiation()
    {
        // Arrange
        loader.Scan(typeof(EmojiPlugin));
        await loader.CheckUpgradeAndRemoveAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var preloadedPlugins = await loader.PreloadAsync();
        stopwatch.Stop();

        // Assert
        Assert.IsTrue(preloadedPlugins.Count > 0, "应该有预加载的插件");
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, "预加载应该很快完成");
        
        // 验证插件已预加载但未实例化
        var preloadedPlugin = loader.GetPreloadedPlugin("Emoji");
        Assert.IsNotNull(preloadedPlugin, "应该能找到预加载的插件");
        Assert.IsFalse(preloadedPlugin.IsInstantiated, "插件应该未实例化");
        Assert.IsNotNull(preloadedPlugin.Assembly, "程序集应该已加载");
        Assert.IsNotNull(preloadedPlugin.MainPluginType, "主插件类型应该已获取");

        // 验证插件实例尚未创建
        var pluginInstance = loader.GetPlugin("Emoji");
        Assert.IsNull(pluginInstance, "插件实例应该尚未创建");
    }

    [TestMethod("两阶段加载 - 延迟实例化阶段")]
    public async Task InstantiationPhase_ShouldCreatePluginInstances()
    {
        // Arrange
        loader.Scan(typeof(EmojiPlugin));
        await loader.CheckUpgradeAndRemoveAsync();
        await loader.PreloadAsync();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var instantiatedCount = await loader.InstantiatePluginsAsync();
        stopwatch.Stop();

        // Assert
        Assert.AreEqual(1, instantiatedCount, "应该实例化1个插件");
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, "实例化应该很快完成");

        // 验证插件实例已创建
        var pluginInstance = loader.GetPlugin("Emoji");
        Assert.IsNotNull(pluginInstance, "插件实例应该已创建");
        Assert.AreEqual("EmojiPlugin", pluginInstance.DisplayName, "插件显示名称应该正确");

        // 验证预加载数据已更新
        var preloadedPlugin = loader.GetPreloadedPlugin("Emoji");
        Assert.IsTrue(preloadedPlugin.IsInstantiated, "预加载数据应该标记为已实例化");
        Assert.IsNotNull(preloadedPlugin.InstantiatedAt, "应该有实例化时间");
    }

    [TestMethod("两阶段加载 - 按需实例化")]
    public async Task OnDemandInstantiation_ShouldOnlyInstantiateSpecifiedPlugins()
    {
        // Arrange
        loader.Scan(typeof(EmojiPlugin));
        await loader.CheckUpgradeAndRemoveAsync();
        await loader.PreloadAsync();

        // Act - 只实例化Emoji插件
        var instantiatedCount = await loader.InstantiatePluginsAsync(new[] { "Emoji" });

        // Assert
        Assert.AreEqual(1, instantiatedCount, "应该只实例化1个指定插件");
        
        var emojiPlugin = loader.GetPlugin("Emoji");
        Assert.IsNotNull(emojiPlugin, "Emoji插件应该已实例化");
    }

    [TestMethod("两阶段加载 - 性能对比")]
    public async Task PerformanceComparison_ShouldShowImprovement()
    {
        // 传统加载方式
        var traditionalLoader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
        traditionalLoader.Scan(typeof(EmojiPlugin));
        await traditionalLoader.CheckUpgradeAndRemoveAsync();

        var traditionalStopwatch = Stopwatch.StartNew();
        await traditionalLoader.LoadAsync();
        traditionalStopwatch.Stop();

        // 两阶段加载方式
        var twoPhaseLoader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
        twoPhaseLoader.Scan(typeof(EmojiPlugin));
        await twoPhaseLoader.CheckUpgradeAndRemoveAsync();

        var preloadStopwatch = Stopwatch.StartNew();
        await twoPhaseLoader.PreloadAsync();
        preloadStopwatch.Stop();

        var instantiateStopwatch = Stopwatch.StartNew();
        await twoPhaseLoader.InstantiatePluginsAsync();
        instantiateStopwatch.Stop();

        var totalTwoPhaseTime = preloadStopwatch.ElapsedMilliseconds + instantiateStopwatch.ElapsedMilliseconds;

        // 验证两阶段加载的总时间不会显著增加
        Assert.IsTrue(totalTwoPhaseTime <= traditionalStopwatch.ElapsedMilliseconds + 100, 
            "两阶段加载总时间不应该显著增加");

        // 验证预加载阶段比传统加载快
        Assert.IsTrue(preloadStopwatch.ElapsedMilliseconds < traditionalStopwatch.ElapsedMilliseconds, 
            "预加载阶段应该比传统加载快");
    }

    [TestMethod("两阶段加载 - 插件状态管理")]
    public async Task PluginStateManagement_ShouldTrackStatesCorrectly()
    {
        // Arrange
        loader.Scan(typeof(EmojiPlugin));
        await loader.CheckUpgradeAndRemoveAsync();

        // Act - 预加载
        var preloadedPlugins = await loader.PreloadAsync();
        var preloadedPlugin = preloadedPlugins.First();

        // Assert - 预加载状态
        Assert.IsFalse(preloadedPlugin.IsInstantiated, "预加载后应该未实例化");
        Assert.IsNull(preloadedPlugin.InstantiatedAt, "预加载后应该没有实例化时间");
        Assert.IsNull(preloadedPlugin.Instance, "预加载后应该没有实例");

        // Act - 实例化
        await loader.InstantiatePluginsAsync();

        // Assert - 实例化状态
        var updatedPreloadedPlugin = loader.GetPreloadedPlugin("Emoji");
        Assert.IsTrue(updatedPreloadedPlugin.IsInstantiated, "实例化后应该标记为已实例化");
        Assert.IsNotNull(updatedPreloadedPlugin.InstantiatedAt, "实例化后应该有实例化时间");
        Assert.IsNotNull(updatedPreloadedPlugin.Instance, "实例化后应该有实例");
    }

    [TestMethod("两阶段加载 - 重复实例化处理")]
    public async Task DuplicateInstantiation_ShouldHandleGracefully()
    {
        // Arrange
        loader.Scan(typeof(EmojiPlugin));
        await loader.CheckUpgradeAndRemoveAsync();
        await loader.PreloadAsync();

        // Act - 第一次实例化
        var firstCount = await loader.InstantiatePluginsAsync();
        
        // Act - 第二次实例化（应该被忽略）
        var secondCount = await loader.InstantiatePluginsAsync();

        // Assert
        Assert.AreEqual(1, firstCount, "第一次实例化应该成功");
        Assert.AreEqual(0, secondCount, "第二次实例化应该被忽略");
        
        var plugin = loader.GetPlugin("Emoji");
        Assert.IsNotNull(plugin, "插件应该存在且只有一个实例");
    }

    [TestMethod("两阶段加载 - 错误处理")]
    public async Task ErrorHandling_ShouldHandlePreloadErrors()
    {
        // Arrange - 扫描不存在的插件
        loader.Scan(new Uri("file:///nonexistent/plugin.json"));
        await loader.CheckUpgradeAndRemoveAsync();

        // Act
        var preloadedPlugins = await loader.PreloadAsync();

        // Assert - 应该处理错误并返回空列表
        Assert.AreEqual(0, preloadedPlugins.Count, "不存在的插件应该被忽略");
    }
}