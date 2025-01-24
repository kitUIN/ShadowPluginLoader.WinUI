using ShadowExample.Core;
using ShadowPluginLoader.MetaAttributes;

namespace ShadowExample.Plugin.Emoji.ViewModels;

public partial class Test2ViewModel: TestViewModel
{
    [Autowired]
    public ShadowExamplePluginLoader ShadowExamplePluginLoader { get; }
    
}