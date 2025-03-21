using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Plugin.Emoji.ViewModels;

public partial class TestViewModel
{
    [Autowired]
    public PluginEventService PluginEventService { get; }
}