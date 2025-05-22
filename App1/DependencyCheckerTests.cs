using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Exceptions;
using System.Collections.Generic;

namespace ShadowPluginLoader.WinUI.Tests.Checkers
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
                Id = id, Version = version, Priority = priority,
                Dependencies = dependencies
            };
            var plugin = new SortPluginData<TestMeta>(meta, "")
            {
                // ∑¥…‰…Ë÷√÷ª∂¡ Ù–‘
            };
            return plugin;
        }

        [TestMethod("≤‚ ‘º”‘ÿÀ≥–Ú")]
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

        [TestMethod("≤‚ ‘”–“¿¿µµƒº”‘ÿ")]
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

        [TestMethod("≤‚ ‘≤ª¥Ê‘⁄“¿¿µ")]
        [ExpectedException(typeof(PluginImportException))]
        public void DetermineLoadOrder_MissingDependency_ThrowsException()
        {
            var checker = new DependencyChecker<TestMeta>();
            var dep = new PluginDependency("X", "1.0.0", "Same");
            var pluginA = CreatePlugin("A", "1.0.0", 1, dep);
            var plugins = new List<SortPluginData<TestMeta>> { pluginA };

            checker.DetermineLoadOrder(plugins);
        }

        [TestMethod("≤‚ ‘“¿¿µ∞Ê±æ≤ª∂‘")]
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