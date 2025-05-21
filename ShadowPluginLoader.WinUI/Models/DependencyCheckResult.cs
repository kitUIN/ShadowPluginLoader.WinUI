using DryIoc;
using System.Collections.Generic;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TMeta"></typeparam>
/// <param name="Result"></param>
/// <param name="NeedUpgradeResult"></param>
public record DependencyCheckResult<TMeta>(
    List<SortPluginData<TMeta>> Result,
    List<SortPluginData<TMeta>> NeedUpgradeResult)
    where TMeta : AbstractPluginMetaData;