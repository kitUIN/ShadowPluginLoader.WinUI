using System;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// Plugin EntryPoint
/// </summary>
public record PluginEntryPoint(string Name, string Type);

/// <summary>
/// EntryPointType
/// </summary>
public record PluginEntryPointType(Type EntryPointType);