using ShadowExample.Core;
using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji.ViewModels;

public partial class Test2ViewModel: TestViewModel
{
    [Autowired]
    public ShadowExamplePluginLoader ShadowExamplePluginLoader { get; }
    
}