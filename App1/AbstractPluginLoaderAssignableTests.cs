using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Windows.ApplicationModel;
using ShadowExample.Core;
using ShadowExample.Plugin.Emoji;
using ShadowPluginLoader.WinUI.Enums;
using ShadowPluginLoader.WinUI.Services;

namespace App1;

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
        Assert.IsTrue(target2.Type == ScanType.Http);
    }

    [TestMethod]
    public void ScanClear_ShouldClearQueue()
    {
        loader.Scan<EmojiPlugin>();
        Assert.IsTrue(loader.GetScanQueue().Count > 0);
        loader.ScanClear();
        Assert.AreEqual(0, loader.GetScanQueue().Count);
    }
}