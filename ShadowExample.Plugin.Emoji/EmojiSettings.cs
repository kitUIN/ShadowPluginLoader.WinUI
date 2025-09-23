using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShadowExample.Plugin.Emoji;

/// <summary>
/// 表情设置类，实现属性变化通知
/// </summary>
public class EmojiSettings : INotifyPropertyChanged
{
    private int _size = 24;
    private bool _enableAnimation = true;
    private string _theme = "default";

    /// <summary>
    /// 表情大小
    /// </summary>
    public int Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 启用动画
    /// </summary>
    public bool EnableAnimation
    {
        get => _enableAnimation;
        set
        {
            if (_enableAnimation != value)
            {
                _enableAnimation = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 主题
    /// </summary>
    public string Theme
    {
        get => _theme;
        set
        {
            if (_theme != value)
            {
                _theme = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
