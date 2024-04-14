using System;
using DryIoc;
using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    public class ShadowExamplePluginLoader : APluginLoader<ExampleMetaData, IExampleMetaData, IExamplePlugin>
    {
        public ShadowExamplePluginLoader(ILogger logger) : base(logger, DiFactory.Services)
        {
        }

        protected override string PluginPrefix => "ShadowExample.Plugin";

        protected override void CheckPluginMetaData(IExampleMetaData meta)
        {
            // Custom Your CheckPluginMetaData Function
            // If Your CheckPluginMetaData Function Throw PluginImportError, The Plugin Will Not Load
        }

        protected override void LoadPluginDi(Type plugin, IExampleMetaData meta)
        {
        }
    }
}