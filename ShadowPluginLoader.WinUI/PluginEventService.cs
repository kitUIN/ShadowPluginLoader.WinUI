using ShadowPluginLoader.WinUI.Args;
using System;
using Serilog;

namespace ShadowPluginLoader.WinUI;

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
}