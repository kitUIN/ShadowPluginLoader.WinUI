using ShadowPluginLoader.Attributes;

namespace ShadowExample.Core.Plugins;

public record InnerMetaData
{
    [Meta]
    public int Number { get; }
    [Meta]
    public double DoubleNumber { get; }
}