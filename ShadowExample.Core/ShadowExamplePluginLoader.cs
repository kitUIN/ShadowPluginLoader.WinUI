using System;
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    public class ShadowExamplePluginLoader : 
        APluginLoader<ExampleMetaData, AExamplePlugin>
    {
        public ShadowExamplePluginLoader(ILogger logger) : base(logger)
        {
        }
        public ShadowExamplePluginLoader() : base()
        {
        }
    }
}