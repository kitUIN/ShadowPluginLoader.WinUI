using System;
using System.Collections.Generic; 
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowExample.Core
{
    public class ShadowExamplePluginLoader :
        AbstractPluginLoader<ExampleMetaData, PluginBase>
    {
        protected override string PluginFolder => "plugins";


        public void ChangeIsCheckUpgradeAndRemove(bool value)
        {
            IsCheckUpgradeAndRemove = value;
        }

        public Queue<Uri> GetScanQueue()
        {
            return ScanQueue;
        }

        public IDependencyChecker<ExampleMetaData> GetDependencyChecker()
        {
            return DependencyChecker;
        }

        public IMetaDataChecker<ExampleMetaData> GetMetaDataChecker()
        {
            return MetaDataChecker;
        }

        /// <inheritdoc />
        protected override string TempFolder => "temps";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pluginEventService"></param>
        public ShadowExamplePluginLoader(ILogger logger, PluginEventService pluginEventService) : base(logger,
            pluginEventService)
        {
        }
    }
}