﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPluginLoader.WinUI.Interfaces;

/// <summary>
/// Default Plugin Interface
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Get identifier of this plugin 
    /// </summary>
    string GetId();

    /// <summary>
    /// Get meta data of this plugin
    /// </summary>
    IPluginMetaData GetMetaData();

    /// <summary>
    /// Know that this Plugin is Enabled/Disabled
    /// </summary>
    bool IsEnabled { get; }
    /// <summary>
    /// Enable this Plugin
    /// </summary>
    void Enable();
    /// <summary>
    /// Disable this Plugin
    /// </summary>
    void Disable();
}
