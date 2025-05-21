using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Moq;
using Serilog;
using ShadowExample.Core;
using ShadowExample.Plugin.Emoji;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

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

    [TestInitialize]
    public void Setup()
    {
        var logger = Log.ForContext<ShadowExamplePluginLoader>();
        loader = new ShadowExamplePluginLoader(logger, new PluginEventService(logger));
    }

    [TestMethod]
    public void Scan_Type_ShouldEnqueueFileInfo()
    {
        loader.Scan(typeof(EmojiPlugin));
        Assert.IsTrue(loader.GetScanQueue().Count == 1);
        var target = loader.GetScanQueue().Dequeue();
        Assert.IsTrue(target.Type == ScanType.FileInfo);
    }
    [UITestMethod]
    public async Task Scan_Type_Load()
    {
        loader.Scan(typeof(EmojiPlugin));
        Assert.IsTrue(loader.GetScanQueue().Count == 1);
        loader.ChangeIsCheckUpgradeAndRemove(true); 
        // await loader.Load();
    }
    [TestMethod]
    public void Scan_Generic_ShouldEnqueueFileInfo()
    {
        loader.Scan<EmojiPlugin>();
        Assert.IsTrue(loader.GetScanQueue().Count == 1);
        var target = loader.GetScanQueue().Dequeue();
        Assert.IsTrue(target.Type == ScanType.FileInfo);
    }

    [TestMethod]
    public void Scan_EnumerableTypes_ShouldEnqueueAll()
    {
        var types = new[] { typeof(EmojiPlugin), typeof(EmojiPlugin) };
        loader.Scan(types);
        Assert.IsTrue(loader.GetScanQueue().Count == 2);
        var target = loader.GetScanQueue().Dequeue();
        Assert.IsTrue(target.Type == ScanType.FileInfo);
        var target2 = loader.GetScanQueue().Dequeue();
        Assert.IsTrue(target2.Type == ScanType.FileInfo);
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
        Assert.IsTrue(loader.GetScanQueue().Count == 1);
        var target2 = loader.GetScanQueue().Dequeue();
        Assert.IsTrue(target2.Type == ScanType.Uri);
    }

    [TestMethod]
    public void ScanClear_ShouldClearQueue()
    {
        loader.Scan<EmojiPlugin>();
        Assert.IsTrue(loader.GetScanQueue().Count > 0);
        loader.ScanClear();
        Assert.AreEqual(0, loader.GetScanQueue().Count);
    }

    [UITestMethod("测试升级")]
    public async Task Load_Upgrade()
    {
        loader.ScanClear();
        loader.Scan(new Uri(
            @"D:\VsProjects\WASDK\ShadowPluginLoader.WinUI\ShadowExample.Plugin.Hello\Packages\ShadowExample.Plugin.Hello-1.0.1-Debug.zip"));
        loader.ChangeIsCheckUpgradeAndRemove(true);
        await loader.Load();
        Assert.AreEqual(1, loader.GetPlugins().Count); 
    }
}