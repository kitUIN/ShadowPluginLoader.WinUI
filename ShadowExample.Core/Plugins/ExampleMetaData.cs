using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core.Plugins;

[ExportMeta]
public record ExampleMetaData : AbstractPluginMetaData
{
    [Meta(Required = false)]
    public string[] Authors { get; init; }

    [Meta(Required = false)]
    public string Url { get; init; }

    [Meta(Required = false)]
    public InnerMetaData InnerMetaData { get; init; }

    [Meta(Required = false)]
    public InnerMetaData[] InnerMetaDatas { get; init; }
}