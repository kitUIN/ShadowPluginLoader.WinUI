using Microsoft.UI.Xaml;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using System.Collections.Generic;
using CustomExtensions.WinUI;
using System;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract Plugin
/// </summary>
public abstract class AbstractPlugin : IPlugin
{
    /// <inheritdoc />
    public abstract string DisplayName { get; }
    /// <summary>
    /// Default
    /// </summary>
    protected AbstractPlugin()
    {
        Init();
    }
    /// <summary>
    /// Init
    /// </summary>
    protected void Init()
    {
        // Load ResourceDictionary
        foreach (var item in ResourceDictionaries)
        {
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri(item.PluginPath())
                });
        }
    }

    /// <summary>
    /// Resource Dictionaries, example: ms-appx:///Themes/BikaTheme.xaml
    /// </summary>
    protected virtual IEnumerable<string> ResourceDictionaries => 
        new List<string>();
    /// <summary>
    /// Is Enabled
    /// </summary>
    private bool _isEnabled;

    /// <inheritdoc/>
    public bool IsEnabled { 
        get => _isEnabled;
        set
        {
            if (value)
            {
                Enable();
                PluginEventService.InvokePluginEnabled(this, 
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.Enabled));
            }
            else
            {
                Disable();
                PluginEventService.InvokePluginDisabled(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.Disabled));
            }
            _isEnabled = value;
            PluginSettingsHelper.SetPluginEnabled(Id, _isEnabled);
        }
    }
    /// <summary>
    /// Disable
    /// </summary>
    protected virtual void Disable()
    {

    }
    /// <summary>
    /// Enable
    /// </summary>
    protected virtual void Enable()
    {
        
    }

    /// <inheritdoc/>
    public abstract string Id { get; }
}
