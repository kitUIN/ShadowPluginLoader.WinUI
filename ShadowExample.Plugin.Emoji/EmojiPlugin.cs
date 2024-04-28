using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;
using ShadowPluginLoader.MetaAttributes;
using Microsoft.UI.Xaml;
using ShadowExample.Plugin.Emoji.Controls;

namespace ShadowExample.Plugin.Emoji
{
    [AutoPluginMeta]
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

        public override string GetId()
        {
            return Meta.Id;
        }
    }
}
