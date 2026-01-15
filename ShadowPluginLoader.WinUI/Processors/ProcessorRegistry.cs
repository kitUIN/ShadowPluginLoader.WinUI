using System.Collections.Generic;
using ShadowPluginLoader.WinUI.Materials;

namespace ShadowPluginLoader.WinUI.Processors;

/// <summary>
/// 处理器注册中心
/// </summary>
public static class ProcessorRegistry
{
    /// <summary>
    /// 预处理处理器
    /// </summary>
    public static readonly Dictionary<string, IPreprocessingProcessor> PreprocessingProcessors =
        new()
        {
            [nameof(LocalFileMaterial)] = new LocalFilePreprocessingProcessor(),
            [nameof(HttpFileMaterial)] = new HttpPreprocessingProcessor(),
            [nameof(CompressedFileMaterial)] = new CompressedFilePreprocessingProcessor(),
        };
}