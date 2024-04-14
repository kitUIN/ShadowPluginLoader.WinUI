using System;

namespace ShadowExample.Core.Plugins;

public class ExampleMetaData: Attribute,IExampleMetaData
{
    public string Id { get; }
    public string Name { get; }
    public string Version { get; }
    public string[] Requires { get; }

    public ExampleMetaData(string id, string name, string version, string[] requires)
    {
        Id = id;
        Name = name;
        Version = version;
        Requires = requires;
    }
}