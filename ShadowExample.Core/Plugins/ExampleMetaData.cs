using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public class ExampleMetaData : AbstractPluginMetaData
{
    [Meta(Required = false)]
    public string[] Authors { get; init; }

    [Meta(Required = false)]
    public string Url { get; init; }

    [Meta(Required = false)]
    public double? D { get; init; }

    [Meta(Required = false)]
    public float[]? F { get; init; }
}