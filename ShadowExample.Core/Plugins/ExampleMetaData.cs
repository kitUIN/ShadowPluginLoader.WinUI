using ShadowPluginLoader.MetaAttributes;
using System;

namespace ShadowExample.Core.Plugins;

public class ExampleMetaData : Attribute, IExampleMetaData
{
    [Meta(Required = true)]
    public string Id { get; init; }

    [Meta(Required = true)]
    public string Name { get; init; }

    [Meta(Required = true)]
    public string Version { get; init; }

    [Meta(Required = true)]
    public string[] Dependencies { get; init; }

}