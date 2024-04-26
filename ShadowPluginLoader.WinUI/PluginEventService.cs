using ShadowPluginLoader.WinUI.Args;
using System;

namespace ShadowPluginLoader.WinUI
{
    /// <summary>
    /// PluginEvent Service
    /// </summary>
    public class PluginEventService
    {
        /// <summary>
        /// Plugin Enabled
        /// </summary>
        static event EventHandler<PluginEventArgs>? PluginEnabled;
        /// <summary>
        /// Plugin Disabled
        /// </summary>
        static event EventHandler<PluginEventArgs>? PluginDisabled;
        /// <summary>
        /// Invoke Plugin Enabled Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void InvokePluginEnabled(object sender, PluginEventArgs args)
        {
            PluginEnabled?.Invoke(sender, args);
        }
        /// <summary>
        /// Invoke Plugin Enabled Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void InvokePluginDisabled(object sender, PluginEventArgs args)
        {
            PluginDisabled?.Invoke(sender, args);
        }
    }
}
