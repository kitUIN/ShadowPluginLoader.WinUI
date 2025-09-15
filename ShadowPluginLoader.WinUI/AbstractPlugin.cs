using Microsoft.UI.Xaml;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Interfaces;
using System.Collections.Generic;
using CustomExtensions.WinUI;
using System;
using Serilog;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Abstract Plugin
/// </summary>
public abstract class AbstractPlugin<TMeta> : IPlugin<TMeta>
    where TMeta : AbstractPluginMetaData
{
    /// <inheritdoc />
    public abstract string DisplayName { get; }

    /// <inheritdoc />
    public virtual TMeta MetaData { get; }

    /// <inheritdoc />
    public virtual string Id => MetaData.Id;
    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// PluginEventService
    /// </summary>
    protected PluginEventService PluginEventService { get; }

    /// <summary>
    /// Default
    /// </summary>
    protected AbstractPlugin(TMeta meta, ILogger logger, PluginEventService pluginEventService)
    {
        MetaData = meta;
        Logger = logger;
        PluginEventService = pluginEventService;
        Init();
    }

    /// <summary>
    /// Init
    /// </summary>
    protected void Init()
    {
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

    /// <summary>
    /// Upgrade Plan
    /// </summary>
    private bool _planUpgrade;

    /// <summary>
    /// Remove Plan
    /// </summary>
    private bool _planRemove;

    /// <inheritdoc/>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            PluginSettingsHelper.SetPluginEnabled(Id, value);
            if (_isEnabled == value) return;
            _isEnabled = value;
            if (value)
            {
                Enabled();
                PluginEventService.InvokePluginEnabled(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.Enabled));
            }
            else
            {
                Disabled();
                PluginEventService.InvokePluginDisabled(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.Disabled));
            }
        }
    }

    /// <inheritdoc/>
    public bool PlanUpgrade
    {
        get => _planUpgrade;
        set
        {
            _planUpgrade = value;
            if (value)
            {
                PluginEventService.InvokePluginPlanUpgrade(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.PlanUpgrade));
            }
            else
            {
                PluginEventService.InvokePluginUpgraded(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.Upgraded));
            }
        }
    }

    /// <inheritdoc/>
    public bool PlanRemove
    {
        get => _planRemove;
        set
        {
            _planRemove = value;
            if (value)
            {
                PluginEventService.InvokePluginPlanRemove(this,
                    new Args.PluginEventArgs(Id, Enums.PluginStatus.PlanRemove));
            }
        }
    }

    /// <summary>
    /// Plugin Disabled (Before Plugin Disabled Event)
    /// </summary>
    protected virtual void Disabled()
    {
    }

    /// <summary>
    /// Plugin Enabled (Before Plugin Enabled Event)
    /// </summary>
    protected virtual void Enabled()
    {
    }

    /// <inheritdoc/>
    public virtual void Loaded()
    {
    }
}