using System.Collections.Generic;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;
using Microsoft.UI.Xaml;
using ShadowExample.Plugin.Emoji.Controls;


namespace ShadowExample.Plugin.Emoji
{
    [MainPlugin]
    [CheckAutowired]
    public partial class EmojiPlugin : PluginBase
    {
        protected override IEnumerable<string> ResourceDictionaries =>
            ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1"];

        public override string DisplayName => "EmojiPlugin";
    }
}