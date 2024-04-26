using Microsoft.UI.Xaml;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using System.Collections.Generic;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract Plugin
/// </summary>
public abstract class APlugin : IPlugin
{
    /// <summary>
    /// Default
    /// </summary>
    public APlugin()
    {
        // Load ResourceDictionary
        foreach (var item in ResourceDictionaries)
        {
            Application.Current.Resources.MergedDictionaries.Add(item);
        }
    }
    /// <summary>
    /// Resource Dictionaries
    /// </summary>
    public virtual IEnumerable<ResourceDictionary> ResourceDictionaries => 
        new List<ResourceDictionary>();

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
