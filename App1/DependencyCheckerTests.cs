using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Exceptions;
using System.Collections.Generic;
using NuGet.Versioning;
using ShadowPluginLoader.WinUI;

namespace App1
{
    [TestClass]
    public class DependencyCheckerTests
    {
        private record TestMeta : AbstractPluginMetaData
        {
        }

        private SortPluginData<TestMeta> CreatePlugin(string id, string version,
            int priority, params PluginDependency[] dependencies)
        {
            var meta = new TestMeta()
            {
                Id = id, Version = new NuGetVersion(version), Priority = priority,
                Dependencies = dependencies
            };
            var plugin = new SortPluginData<TestMeta>(meta, "C:/test","Base")
            {
                // 反射设置只读属性
            };
            return plugin;
        }

        [TestMethod("依赖加载顺序")]
        public void DetermineLoadOrder_NoDependencies_ReturnsPluginsInPriorityOrder()
        {
            var checker = new DependencyChecker<TestMeta>();
            var pluginA = CreatePlugin("A", "1.0.0", 1);
            var pluginB = CreatePlugin("B", "1.0.0", 2);
            var plugins = new List<SortPluginData<TestMeta>> { pluginB, pluginA };

            var result = checker.DetermineLoadOrder(plugins);

            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual("A", result.Result[0].Id);
            Assert.AreEqual("B", result.Result[1].Id);
        }

        [TestMethod("有依赖的加载")]
        public void DetermineLoadOrder_WithDependencies_RespectsDependencyOrder()
        {
            var checker = new DependencyChecker<TestMeta>();
            var dep = new PluginDependency("A", "1.0.0", "Same");
            var pluginA = CreatePlugin("A", "1.0.0", 1);
            var pluginB = CreatePlugin("B", "1.0.0", 2, dep);
            var plugins = new List<SortPluginData<TestMeta>> { pluginB, pluginA };

            var result = checker.DetermineLoadOrder(plugins);

            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual("A", result.Result[0].Id);
            Assert.AreEqual("B", result.Result[1].Id);
        }

        [TestMethod("不存在依赖")]
        [ExpectedException(typeof(PluginImportException))]
        public void DetermineLoadOrder_MissingDependency_ThrowsException()
        {
            var checker = new DependencyChecker<TestMeta>();
            var dep = new PluginDependency("X", "1.0.0", "Same");
            var pluginA = CreatePlugin("A", "1.0.0", 1, dep);
            var plugins = new List<SortPluginData<TestMeta>> { pluginA };

            checker.DetermineLoadOrder(plugins);
        }

        [TestMethod("依赖版本错误")]
        [ExpectedException(typeof(PluginImportException))]
        public void DetermineLoadOrder_VersionNotSatisfied_ThrowsException()
        {
            var checker = new DependencyChecker<TestMeta>();
            var dep = new PluginDependency("A", "2.0.0", "Same");
            var pluginA = CreatePlugin("A", "1.0.0", 1);
            var pluginB = CreatePlugin("B", "1.0.0", 2, dep);
            var plugins = new List<SortPluginData<TestMeta>> { pluginA, pluginB };

            checker.DetermineLoadOrder(plugins);
        }
    }
}