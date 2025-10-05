using System;
using ShadowObservableConfig.Attributes;
using System.Collections.ObjectModel;

namespace Config.WinUI;

/// <summary>
/// 表情渲染模式枚举
/// </summary>
public enum EmojiRenderMode
{
    /// <summary>
    /// 原生渲染
    /// </summary>
    Native,
    
    /// <summary>
    /// Twemoji渲染
    /// </summary>
    Twemoji,
    
    /// <summary>
    /// Segoe UI渲染
    /// </summary>
    SegoeUi,
    
    /// <summary>
    /// Apple渲染
    /// </summary>
    Apple,
    
    /// <summary>
    /// Google渲染
    /// </summary>
    Google
}

/// <summary>
/// Emoji插件配置类
/// </summary>
[ObservableConfig(FileName = "emoji_config", DirPath = "config", Description = "Emoji插件配置", Version = "1.0.0")]
public partial class EmojiConfig
{
    [ObservableConfigProperty(Description = "默认表情大小")]
    private int defaultEmojiSize;

    [ObservableConfigProperty(Description = "启用自动完成")]
    private bool _enableAutoComplete;

    [ObservableConfigProperty(Name = "MaxEmojiHistory", Description = "最大表情历史记录数")]
    private int _maxEmojiHistory;

    [ObservableConfigProperty(Name = "DefaultSkinTone", Description = "默认肤色")]
    private string _defaultSkinTone = null!;

    [ObservableConfigProperty(Name = "AnimationSpeed", Description = "动画速度")]
    private double _animationSpeed;

    [ObservableConfigProperty(Name = "RenderMode", Description = "表情渲染模式")]
    private EmojiRenderMode _renderMode;

    [ObservableConfigProperty(Name = "LaunchDate", Description = "启用日期")]
    private DateTime _launchDate;

    [ObservableConfigProperty(Name = "Settings", Description = "表情设置")]
    private NestedSettings _settings = new();

    [ObservableConfigProperty(Name = "FavoriteEmojis", Description = "收藏的表情列表")]
    private ObservableCollection<string> _favoriteEmojis = new ();

    [ObservableConfigProperty(Name = "CustomSettings", Description = "自定义设置列表")]
    private ObservableCollection<NestedSettings> _customSettings = new();
}

/// <summary>
/// 内部配置类示例
/// </summary>
[ObservableConfig(Description = "嵌套配置", Version = "1.0.0")]
public partial class NestedSettings
{
    [ObservableConfigProperty(Name = "NestedValue", Description = "嵌套值")]
    private string _nestedValue;

    [ObservableConfigProperty(Name = "NestedNumber", Description = "嵌套数字")]
    private int _nestedNumber;

    [ObservableConfigProperty(Name = "NestedBoolean", Description = "嵌套布尔值")]
    private bool _nestedBoolean;

}