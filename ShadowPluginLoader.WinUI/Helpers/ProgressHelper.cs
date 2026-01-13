namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// 
/// </summary>
public static class ProgressHelper
{
    /// <summary>
    /// 将子任务进度映射到全局进度区间
    /// </summary>
    /// <param name="currentStepIndex">当前处于第几个子任务</param>
    /// <param name="totalSteps">子任务总数</param>
    /// <param name="rangeWidth">全局进度条中该阶段所占的总宽度 (例如 40 表示占用 40%)</param>
    /// <param name="rangeStart">全局进度条的起始位置 (0-100)</param>
    public static (double, double) CreateSubProgressBegin(
        int currentStepIndex,
        int totalSteps,
        double rangeWidth,
        double rangeStart = 0D)
    {
        // 计算当前子任务在全局进度中的起始偏移量
        double baseOffset = rangeStart + ((double)currentStepIndex / totalSteps * rangeWidth);
        // 计算每个子任务在全局进度中占据的最大权重
        double weight = (1.0 / totalSteps) * rangeWidth;
        return (baseOffset, weight);
    }
}