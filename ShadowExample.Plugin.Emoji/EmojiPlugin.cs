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
    public partial class EmojiPlugin: IExamplePlugin
    {
        public static string Id => "emoji";
        public string GetId()
        {
            return Id;
        }

        public AbstractPluginMetaData GetMetaData()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; set; }
        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public string GetEmoji()
        {
            throw new NotImplementedException();
        }
    }
}
