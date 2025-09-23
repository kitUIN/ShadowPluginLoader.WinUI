using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

/// <summary>
/// Emoji插件配置类
/// </summary>
[Config(FileName = "emoji_config.json", DirPath = "config", Description = "Emoji插件配置", Version = "1.0.0")]
public partial class EmojiConfig
{
    [ConfigField(Name = "DefaultEmojiSize", Description = "默认表情大小")]
    private int _defaultEmojiSize;

    [ConfigField(Name = "EnableAutoComplete", Description = "启用自动完成")]
    private bool _enableAutoComplete;

    [ConfigField(Name = "MaxEmojiHistory", Description = "最大表情历史记录数")]
    private int _maxEmojiHistory;

    [ConfigField(Name = "DefaultSkinTone", Description = "默认肤色")]
    private string _defaultSkinTone;

    [ConfigField(Name = "AnimationSpeed", Description = "动画速度")]
    private double _animationSpeed;

    [ConfigField(Name = "Settings", Description = "表情设置")]
    private EmojiSettings _settings;

    /// <summary>
    /// 配置初始化后的回调
    /// </summary>
    partial void AfterConfigInit()
    {
        // 初始化实体类
        _settings ??= new EmojiSettings();
        
        // 在这里可以添加配置初始化后的逻辑
        System.Diagnostics.Debug.WriteLine($"EmojiConfig initialized: Size={DefaultEmojiSize}, AutoComplete={EnableAutoComplete}");
    }
}
