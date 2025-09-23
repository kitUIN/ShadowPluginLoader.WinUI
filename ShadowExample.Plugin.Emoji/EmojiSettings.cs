using ShadowPluginLoader.WinUI.Models;

namespace ShadowExample.Plugin.Emoji;

/// <summary>
/// 表情设置实体类
/// </summary>
public partial class EmojiSettings : BaseConfig
{
    private string _theme;
    private bool _enableAnimations;
    private int _maxRecentCount;
    private NestedSettings _nestedSettings;

    /// <summary>
    /// 内部类不保存单独文件
    /// </summary>
    protected override string FileName => "";

    /// <summary>
    /// 内部类不保存单独文件
    /// </summary>
    protected override string ConfigPath => "";

    /// <summary>
    /// 内部类不保存单独文件
    /// </summary>
    protected override string DirectoryName => "";

    /// <summary>
    /// 主题设置
    /// </summary>
    public string Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    /// <summary>
    /// 是否启用动画
    /// </summary>
    public bool EnableAnimations
    {
        get => _enableAnimations;
        set => SetProperty(ref _enableAnimations, value);
    }

    /// <summary>
    /// 最大最近使用数量
    /// </summary>
    public int MaxRecentCount
    {
        get => _maxRecentCount;
        set => SetProperty(ref _maxRecentCount, value);
    }

    /// <summary>
    /// 嵌套设置
    /// </summary>
    public NestedSettings NestedSettings
    {
        get => _nestedSettings;
        set => SetProperty(ref _nestedSettings, value);
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public EmojiSettings()
    {
        _theme = "default";
        _enableAnimations = true;
        _maxRecentCount = 20;
        _nestedSettings = new NestedSettings();
    }

    /// <summary>
    /// 内部配置类示例
    /// </summary>
    [Config(Description = "嵌套配置", Version = "1.0.0")]
    public partial class NestedSettings : BaseConfig
    {
        [ConfigField(Name = "NestedValue", Description = "嵌套值")]
        private string _nestedValue;

        [ConfigField(Name = "NestedNumber", Description = "嵌套数字")]
        private int _nestedNumber;

        [ConfigField(Name = "NestedBoolean", Description = "嵌套布尔值")]
        private bool _nestedBoolean;

        /// <summary>
        /// 配置初始化后的回调
        /// </summary>
        partial void AfterConfigInit()
        {
            // 在这里可以添加嵌套配置初始化后的逻辑
            System.Diagnostics.Debug.WriteLine($"NestedSettings initialized: Value={NestedValue}, Number={NestedNumber}");
        }
    }
}