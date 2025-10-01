namespace ShadowPluginLoader.SourceGenerator.Models;

/// <summary>
/// Base Constructor Parameter
/// </summary>
internal class BaseConstructor
{
    /// <summary>
    /// Parameter Type
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Parameter Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// BaseConstructor
    /// </summary>
    /// <param name="type">Parameter Type</param>
    /// <param name="name">Parameter Name</param>
    public BaseConstructor(string type, string name)
    {
        Type = type;
        Name = name;
    }
}
