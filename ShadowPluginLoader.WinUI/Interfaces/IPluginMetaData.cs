using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces
{
    public interface IPluginMetaData
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        string[] Requires { get; }
    }
}
