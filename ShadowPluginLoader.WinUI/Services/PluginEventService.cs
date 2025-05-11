using System;
using Serilog;
using ShadowPluginLoader.WinUI.Args;

namespace ShadowPluginLoader.WinUI.Services;

/// <summary>
/// PluginEvent Service
/// </summary>
public class PluginEventService
{
    private ILogger Logger { get; }

    /// <summary>
    /// PluginEvent Service
    /// </summary>
    public PluginEventService(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Plugin Enabled
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginEnabled;

    /// <summary>
    /// Plugin Disabled
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginDisabled;

    /// <summary>
    /// Plugin Loaded
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginLoaded;


    /// <summary>
    /// Plugin Plan Upgrade
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginPlanUpgrade;

    /// <summary>
    /// Plugin Upgraded
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginUpgraded;

    /// <summary>
    /// Plugin Plan Remove
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginPlanRemove;

    /// <summary>
    /// Plugin Removed
    /// </summary>
    public event EventHandler<PluginEventArgs>? PluginRemoved;

    /// <summary>
    /// Invoke Plugin Enabled Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginEnabled(object sender, PluginEventArgs args)
    {
        PluginEnabled?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Enabled Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Enabled Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginDisabled(object sender, PluginEventArgs args)
    {
        PluginDisabled?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Disabled Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Loaded Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginLoaded(object sender, PluginEventArgs args)
    {
        PluginLoaded?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Loaded Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Plan Upgrade Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginPlanUpgrade(object sender, PluginEventArgs args)
    {
        PluginPlanUpgrade?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Plan Upgrade Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Upgraded Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginUpgraded(object sender, PluginEventArgs args)
    {
        PluginUpgraded?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Upgraded Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Plan Remove Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginPlanRemove(object sender, PluginEventArgs args)
    {
        PluginPlanRemove?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Plan Remove Event",
            sender.GetType().Name, args.PluginId);
    }

    /// <summary>
    /// Invoke Plugin Removed Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void InvokePluginRemoved(object sender, PluginEventArgs args)
    {
        PluginRemoved?.Invoke(sender, args);
        Logger.Debug("Sender {sender} Invoke {id} Plugin Removed Event",
            sender.GetType().Name, args.PluginId);
    }
}