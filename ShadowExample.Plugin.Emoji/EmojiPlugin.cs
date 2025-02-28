﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.MetaAttributes;
using Microsoft.UI.Xaml;
using Serilog;
using ShadowExample.Plugin.Emoji.Controls;
using ShadowPluginLoader.WinUI;


namespace ShadowExample.Plugin.Emoji
{
    [AutoPluginMeta]
    public partial class EmojiPlugin : PluginBase
    {
        /// <inheritdoc />
        public EmojiPlugin(ILogger logger, PluginEventService pluginEventService) : base(logger, pluginEventService)
        {
        }

        public override FrameworkElement GetControl()
        {
            return new UserControl1();
        }

        public override string GetEmoji()
        {
            return "💡😭";
        }
 
        public override string DisplayName => "";
    }
}
