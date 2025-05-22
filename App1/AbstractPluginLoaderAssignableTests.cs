using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Serilog;
using ShadowExample.Core;
using ShadowExample.Plugin.Emoji;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Services;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace App1;

public class MySynchronizationContext : SynchronizationContext
{
    private readonly Queue<(SendOrPostCallback d, object? state)> _queue = new();

    public MySynchronizationContext()
    {
        new Thread(() =>
        {
            SetSynchronizationContext(this);
            while (true)
            {
                if (_queue.TryDequeue(out var result))
                {
                    result.d(result.state);
                }
            }
        })
        {
            Name = "Sync context thread",
        }.Start();
    }

    public override SynchronizationContext CreateCopy()
        => new MySynchronizationContext();

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Enqueue((d, state));
    }

    public override void Send(SendOrPostCallback d, object? state)
        => Post(d, state);
}

public class MyTestMethodAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        TestResult result = null;
        var tcs = new TaskCompletionSource();
        var context = new MySynchronizationContext();
        context.Post(_ =>
        {
            // This Invoke is called on the "Sync context thread" thread.
            // When we "Invoke", MSTest will do GetAwaiter().GetResult() in InvokeAsSynchronousTask.
            // So, we are blocking the "Sync context thread" thread here.
            result = testMethod.Invoke(null);
            tcs.TrySetResult();
        }, null);

        tcs.Task.GetAwaiter().GetResult();
        return new TestResult[] { result };
    }
}

[TestClass]
public class AbstractPluginLoaderAssignableTests
{
    private ShadowExamplePluginLoader loader;
    private ILogger logger;

    [TestInitialize]
    public void Setup()
    {
        logger = Log.ForContext<ShadowExamplePluginLoader>();
        loader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
    }

    [TestMethod]
    public void Scan_Type_ShouldEnqueueFileInfo()
    {
        loader.Scan(typeof(EmojiPlugin));
        Assert.AreEqual(1, loader.GetScanQueue().Count);
        var target = loader.GetScanQueue().Dequeue();
    }

    [UITestMethod]
    public async Task Scan_Type_Load()
    {
        loader.Scan(typeof(EmojiPlugin));
        Assert.AreEqual(1, loader.GetScanQueue().Count);
        loader.ChangeIsCheckUpgradeAndRemove(true);
        // await loader.LoadAsync();
    }

    [TestMethod]
    public void Scan_Generic_ShouldEnqueueFileInfo()
    {
        loader.Scan<EmojiPlugin>();
        Assert.AreEqual(1, loader.GetScanQueue().Count);
        var target = loader.GetScanQueue().Dequeue();
    }

    [TestMethod]
    public void Scan_EnumerableTypes_ShouldEnqueueAll()
    {
        var types = new[] { typeof(EmojiPlugin), typeof(EmojiPlugin) };
        loader.Scan(types);
        Assert.AreEqual(2, loader.GetScanQueue().Count);
        var target = loader.GetScanQueue().Dequeue();
        var target2 = loader.GetScanQueue().Dequeue();
    }

    [TestMethod]
    public void Scan_DirectoryInfo_ShouldEnqueueAll()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        loader.Scan(dir);
        Assert.IsTrue(loader.GetScanQueue().Count >= 0);
    }

    [TestMethod]
    public void Scan_FileInfo_ShouldEnqueue()
    {
        var file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "plugin.json"));
        loader.Scan(file);
        Assert.IsTrue(loader.GetScanQueue().Count > 0);
    }

    [TestMethod]
    public void Scan_Uri_File_ShouldEnqueue()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        var uri = new Uri(dir.FullName + Path.DirectorySeparatorChar, UriKind.Absolute);
        loader.Scan(uri);
        Assert.IsTrue(loader.GetScanQueue().Count >= 0);
    }

    [TestMethod]
    public void Scan_Uri_Http_ShouldEnqueue()
    {
        var uri = new Uri("http://localhost/plugin.json");
        loader.Scan(uri);
        Assert.AreEqual(1, loader.GetScanQueue().Count);
        var target2 = loader.GetScanQueue().Dequeue();
    }

    [TestMethod]
    public void ScanClear_ShouldClearQueue()
    {
        loader.Scan<EmojiPlugin>();
        Assert.IsTrue(loader.GetScanQueue().Count > 0);
        loader.ScanClear();
        Assert.AreEqual(0, loader.GetScanQueue().Count);
    }

    [UITestMethod("同一插件应当提醒不重复加载")]
    public async Task Load_Upgrade()
    {
        var target1 =
            Path.GetFullPath(@"ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.0.1-Debug.zip");
        var target2 = Path.GetFullPath(@"ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.1-Debug.zip");
        var pluginLoader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
        pluginLoader.Scan(new Uri(target1));
        pluginLoader.ChangeIsCheckUpgradeAndRemove(true);
        var res = await pluginLoader.LoadAsync();
        Assert.AreEqual(0, res.Count);
        Assert.AreEqual(1, pluginLoader.GetPlugins().Count);
        Assert.AreEqual("Hello", pluginLoader.GetPlugins()[0].Id);
        pluginLoader.Scan(new Uri(target2));
        res = await pluginLoader.LoadAsync();
        Assert.AreEqual(1, res.Count);
    }

    private void Unzip(string target1, string target)
    {
        var options = new ReaderOptions
        {
            LeaveStreamOpen = false,
            ArchiveEncoding = new ArchiveEncoding
            {
                Default = Encoding.UTF8
            }
        };
        using var archive = ArchiveFactory.Open(target1, options);
        if (!Directory.Exists(target)) Directory.CreateDirectory(target);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            entry.WriteToDirectory(target, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true,
            });
        }
    }

    [UITestMethod("插件更新")]
    public async Task Upgrade_Plugin()
    {
        var target1 =
            Path.GetFullPath(@"ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.0.1-Debug.zip");
        var target = Path.GetFullPath(@"plugins\ShadowExample.Plugin.Hello-1.0.1-Debug8");
        Unzip(target1, target);
        var pluginLoader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
        PluginSettingsHelper.SetPluginUpgradePath("Hello",
            Path.GetFullPath(@"ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.1-Debug.zip"),
            target);
        await pluginLoader.CheckUpgradeAndRemoveAsync();
        var res = await pluginLoader.Scan(new DirectoryInfo(target)).LoadAsync();
        Assert.AreEqual(0, res.Count);
        Assert.AreEqual(1, pluginLoader.GetPlugins().Count);
        Assert.AreEqual("1.1", pluginLoader.GetPlugins()[0].MetaData.Version);
    }

    [UITestMethod("插件删除")]
    public async Task Remove_Plugin()
    {
        var target1 =
            Path.GetFullPath(@"ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.0.1-Debug.zip");
        var target = Path.GetFullPath(@"plugins\ShadowExample.Plugin.Hello-1.0.1-Debug8");
        Unzip(target1, target);
        var pluginLoader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
        PluginSettingsHelper.SetPluginPlanRemove("Hello", target);
        await pluginLoader.CheckUpgradeAndRemoveAsync();
        Assert.IsFalse(Directory.Exists(target));
    }
}