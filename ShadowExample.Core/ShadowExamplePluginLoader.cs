using Serilog;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core
{
    public class ShadowExamplePluginLoader :
        AbstractPluginLoader<ExampleMetaData, PluginBase>
    {
        protected override string PluginFolder => "plugins";

        /// <inheritdoc />
        protected override string TempFolder => "temps";

        public ShadowExamplePluginLoader(ILogger logger, PluginEventService pluginEventService) : base(logger,
            pluginEventService)
        {
        }
    }
}