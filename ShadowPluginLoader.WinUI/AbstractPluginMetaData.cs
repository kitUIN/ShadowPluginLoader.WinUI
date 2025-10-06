using NuGet.Versioning;
using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract PluginMetaData
/// </summary>
public abstract record AbstractPluginMetaData : IPluginMetaData
{
    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Id"/>
    /// </summary>
    [Meta(Required = true)]
    public string Id { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Name"/>
    /// </summary>
    [Meta(Required = true)]
    public string Name { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.BuiltIn"/>
    /// </summary>
    [Meta(Exclude = true)]
    public bool BuiltIn { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.DllName"/>
    /// </summary>
    [Meta(Exclude = true)]
    public string DllName { get; init; } = null!;


    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Version"/>
    /// </summary>
    [Meta(Required = true, AsString = true)]
    public NuGetVersion Version { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.SdkVersion"/>
    /// </summary>
    [Meta(Required = false, AsString = true)]
    public NuGetVersion SdkVersion { get; init; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.MainPlugin"/>
    /// </summary>
    [Meta(Exclude = true)]
    public PluginEntryPointType MainPlugin { get; private set; } = null!;

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Priority"/>
    /// </summary>
    [Meta(Required = false)]
    public int Priority { get; init; }

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.Dependencies"/>
    /// </summary>
    [Meta(Exclude = true, AsString = true)]
    public PluginDependency[] Dependencies { get; init; } = [];

    /// <summary>
    /// <inheritdoc cref="IPluginMetaData.EntryPoints"/>
    /// </summary>
    [Meta(Exclude = true)]
    public PluginEntryPoint[] EntryPoints { get; init; } = [];

    /// <summary>
    /// 
    /// </summary>
    private static readonly Type TargetTypeList = typeof(PluginEntryPointType[]);

    /// <summary>
    /// LoadEntryPoint
    /// </summary>
    /// <returns></returns>
    public void LoadEntryPoint(PropertyPath[] propertyPaths, Assembly assembly)
    {
        foreach (var path in propertyPaths)
        {
            object? current = this;
            for (var i = 0; i < path.Path.Count - 1; i++)
            {
                var prop = path.Path[i];
                var next = prop.GetValue(current);
                if (next == null)
                {
                    next = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(current, next);
                }
                current = next;
            }

            var targetProp = path.TargetProperty;
            var isList = targetProp.PropertyType == TargetTypeList;

            if (isList)
            {
                List<PluginEntryPointType> entryPoints = [];
                foreach (var entryPoint in EntryPoints)
                {
                    if (entryPoint.Name == targetProp.Name && assembly.GetType(entryPoint.Type) is { } type)
                        entryPoints.Add(new PluginEntryPointType(type));
                }
                targetProp.SetValue(current, entryPoints.ToArray());
            }
            else
            {
                PluginEntryPointType? entryPoint = null;
                foreach (var ep in EntryPoints)
                {
                    if (ep.Name != targetProp.Name || assembly.GetType(ep.Type) is not { } type) continue;
                    entryPoint = new PluginEntryPointType(type);
                    break;
                }
                targetProp.SetValue(current, entryPoint);
            }
        }
    }
}