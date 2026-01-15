using System.Collections.Generic;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.Attributes;


namespace ShadowExample.Plugin.Emoji
{
    [MainPlugin]
    [CheckAutowired]
    public partial class EmojiPlugin : PluginBase
    {
        protected override IEnumerable<string> ResourceDictionaries =>
            ["ms-plugin://ShadowExample.Plugin.Emoji/Themes/ResourceDictionary1.xaml"];
        public override string DisplayName => "EmojiPlugin";
    }
}