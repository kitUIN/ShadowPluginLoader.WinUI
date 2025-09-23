using System.ComponentModel;

namespace ShadowPluginLoader.WinUI.Models;

/// <summary>
/// 支持变更通知的实体类接口
/// 用于内部实体类，当内部属性变更时能够通知外部配置类
/// </summary>
public interface IEntityWithChangeNotification : INotifyPropertyChanged
{
    /// <summary>
    /// 实体变更事件，当内部属性发生变更时触发
    /// </summary>
    event EventHandler<EntityChangedEventArgs> EntityChanged;
    
    /// <summary>
    /// 触发实体变更事件
    /// </summary>
    /// <param name="propertyName">变更的属性名</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    void OnEntityChanged(string propertyName, object oldValue, object newValue);
}

/// <summary>
/// 实体变更事件参数
/// </summary>
/// <param name="PropertyName">变更的属性名</param>
/// <param name="FullPropertyPath">完整的属性路径（从根对象开始的完整路径）</param>
/// <param name="OldValue">旧值</param>
/// <param name="NewValue">新值</param>
/// <param name="EntityType">实体类型</param>
public record EntityChangedEventArgs(
    string PropertyName,
    string FullPropertyPath,
    object OldValue,
    object NewValue,
    Type EntityType
);
