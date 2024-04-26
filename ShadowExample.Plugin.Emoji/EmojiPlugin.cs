using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.SourceGenerator.Attributes;
using ShadowPluginLoader.WinUI.Interfaces;
using ShadowPluginLoader.WinUI.Models;

namespace ShadowExample.Plugin.Emoji
{
    [AutoPluginMeta]
    public partial class EmojiPlugin: AExamplePlugin
    {
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
