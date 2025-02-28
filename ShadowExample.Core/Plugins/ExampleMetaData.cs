using ShadowPluginLoader.MetaAttributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public class ExampleMetaData : AbstractPluginMetaData
{
    [Meta()]
    public string[] Authors { get; init; } = ["2","3"];

    [Meta(Required = false)] public string Url { get; init; } = "https://";

    [Meta(Required = false)]
    public double? D { get; init; }

    [Meta(Required = false)]
    public float[]? F { get; init; }

}