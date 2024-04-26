using ShadowPluginLoader.MetaAttributes;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public class ExampleMetaData : AbstractPluginMetaData
{
    [Meta(Required = true, PropertyGroupName = "Author")]
    public string Author { get; init; }
}