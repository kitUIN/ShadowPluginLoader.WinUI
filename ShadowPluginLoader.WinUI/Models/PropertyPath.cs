using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShadowPluginLoader.WinUI.Models;

public class PropertyPath
{
    public List<PropertyInfo> Path { get; }

    public PropertyPath(List<PropertyInfo> path)
    {
        Path = path;
    }

    public PropertyInfo TargetProperty => Path.Last();
}
