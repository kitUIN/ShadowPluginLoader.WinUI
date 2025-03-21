using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.Attributes;
using Microsoft.UI.Xaml;
using Serilog;
using ShadowExample.Plugin.Emoji.Controls;
using ShadowPluginLoader.WinUI;


namespace ShadowExample.Plugin.Emoji
{
    [AutoPluginMeta]
    [CheckAutowired]
    public partial class EmojiPlugin : PluginBase
    {
         
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
