using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShadowPluginLoader.WinUI.Materials;
using ShadowPluginLoader.WinUI.Workpieces;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 预加工处理器
/// 原料<see cref="IMaterial"/>预加工到空白工件<see cref="IWorkpiece"/>
/// </summary>
public interface IPreprocessingProcessor
{
    /// <summary>
    /// 预处理
    /// </summary>
    /// <returns>空白工件<see cref="IWorkpiece"/></returns>
    Task<IWorkpiece> PreprocessAsync(IMaterial material, CancellationToken cancellationToken = default);
}