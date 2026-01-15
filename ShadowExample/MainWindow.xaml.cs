using DryIoc;
using Microsoft.UI.Xaml;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Services;
using ShadowPluginLoader.WinUI.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowExample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Loader = DiFactory.Services.Resolve<ShadowExamplePluginLoader>();
            PluginEventService.PluginLoaded += OnPluginLoaded;
            Init();
        }

        private void OnPluginLoaded(object sender, PluginEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                Block.Text += $"Success: {e.PluginId}[{Loader.GetPlugin(e.PluginId)?.MetaData.Version}]\n";
            });
        }

        public ShadowExamplePluginLoader Loader { get; }
        public PluginEventService PluginEventService = DiFactory.Services.Resolve<PluginEventService>();

        private async void Init()
        {
            await Loader.CheckUpgradeAndRemoveAsync();
            var session = Loader.CreatePipeline();
            await session
                .Feed(new DirectoryInfo(Loader.PluginFolderPath))
                .ProcessAsync();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }

        private async void InstallButton_OnClick(object sender, RoutedEventArgs e)
        {
            var name =
                @"ShadowExample.Plugin.Emoji-1.0.2.9-Debug.sdow";
            await Loader.CreatePipeline()
                .Feed(new Uri(Path.Combine(AppContext.BaseDirectory, "../../../../../../", "package", name)))
                .ProcessAsync();
        }

        private async void Install2Button_OnClick(object sender, RoutedEventArgs e)
        {
            var name =
                @"ShadowExample.Plugin.Hello-1.1.4-Debug.sdow";
            await Loader.CreatePipeline()
                .Feed(new Uri(Path.Combine(AppContext.BaseDirectory, "../../../../../../", "package", name)))
                .ProcessAsync();
        }

        private async void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pluginId = "ShadowExample.Plugin.Emoji";
            await Loader.RemovePlugin(pluginId);
            RemoveButton.Content = "Remove";
            RebootButton.IsEnabled = true;
        }

        private void RebootButton_OnClick(object sender, RoutedEventArgs e)
        {
            var appUserModelId = "92a0d0f6-de67-47d2-858a-dc8e0e56bdb1_ssncz3j6xqbzw!App";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c timeout /t 3 & explorer.exe shell:AppsFolder\\{appUserModelId}",
                CreateNoWindow = true,
                UseShellExecute = false
            });

            Environment.Exit(0);
        }

        private async void UpgradeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pluginId = "ShadowExample.Plugin.Emoji";
            var name =
                @"ShadowExample.Plugin.Emoji-1.0.2.10-Debug.sdow";
            await Loader.UpgradePlugin(pluginId, new Uri(Path.Combine(AppContext.BaseDirectory, "../../../../../../", "package", name)));
            UpgradeButton.Content = "Upgrade";
            RebootButton.IsEnabled = true;
        }
    }
}