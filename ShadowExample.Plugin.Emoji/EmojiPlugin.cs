using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI.Interfaces;

namespace ShadowExample.Plugin.Emoji
{
    public class EmojiPlugin:IExamplePlugin
    {
        public static string Id => "emoji";
        public string GetId()
        {
            return Id;
        }

        public IPluginMetaData GetMetaData()
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
