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
    /// <summary>
    /// Default
    /// </summary>
    public AbstractPlugin()
    {
        // Load ResourceDictionary
        foreach (var item in ResourceDictionaries)
        {
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { 
                    Source = new Uri(item.AssetPath(this.GetType()))
                });
        }
    }
    /// <summary>
    /// Resource Dictionaries, example: ms-appx:///Themes/BikaTheme.xaml
    /// </summary>
    public virtual IEnumerable<string> ResourceDictionaries => 
        new List<string>();

    private bool isEnabled;
    /// <inheritdoc/>
    public bool IsEnabled { 
        get => isEnabled;
        set
        {
            if (value)
            {
                Enable();
                PluginEventService.InvokePluginEnabled(this, 
                    new Args.PluginEventArgs(GetId(),
                    Enums.PluginStatus.Enabled));
            }
            else
            {
                Disable();
                PluginEventService.InvokePluginDisabled(this,
                    new Args.PluginEventArgs(GetId(),
                    Enums.PluginStatus.Disabled));
            }
            isEnabled = value;
            PluginSettingsHelper.SetPluginEnabled(GetId(), isEnabled);
        }
    }
    /// <inheritdoc/>
    protected virtual void Disable()
    {

    }
    /// <inheritdoc/>
    protected virtual void Enable()
    {
        
    }
    /// <inheritdoc/>
    public abstract string GetId();
}
