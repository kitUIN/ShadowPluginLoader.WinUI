using ShadowPluginLoader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowExample.Core.Plugins;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowExample.Plugin.Hello
{
    [MainPlugin]
    [CheckAutowired]
    public partial class HelloPlugin : PluginBase
    {
        protected override IEnumerable<string> ResourceDictionaries =>
            ["ms-plugin://ShadowExample.Plugin.Hello/Themes/ResourceDictionary1.xaml"];

        public override string DisplayName => "HelloPlugin";
    }
}
