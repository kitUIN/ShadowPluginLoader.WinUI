using ShadowPluginLoader.WinUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Args;

/// <summary>
/// Plugin Event Args
/// </summary>
public class PluginEventArgs
{
    /// <summary>
    /// Plugin Id
    /// </summary>
    public string PluginId { get; set; }
    /// <summary>
    /// Plugin Status
    /// </summary>
    public PluginStatus Status { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">PluginId</param>
    /// <param name="status">PluginStatus</param>
    public PluginEventArgs(string id, PluginStatus status)
    {
        PluginId = id;
        Status = status;
    }
}
