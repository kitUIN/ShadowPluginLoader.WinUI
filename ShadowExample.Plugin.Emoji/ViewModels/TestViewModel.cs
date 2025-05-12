using ShadowPluginLoader.Attributes;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowExample.Plugin.Emoji.ViewModels;

public partial class TestViewModel
{
    [Autowired]
    public PluginEventService PluginEventService { get; }
}