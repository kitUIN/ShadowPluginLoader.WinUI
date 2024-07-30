using System;
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    public class ShadowExamplePluginLoader : 
        AbstractPluginLoader<ExampleMetaData, PluginBase>
    {
        protected override string PluginFolder => "plugins";

        public ShadowExamplePluginLoader(ILogger logger) : base(logger)
        {
        }
        public ShadowExamplePluginLoader() : base()
        {
        }
    }
}