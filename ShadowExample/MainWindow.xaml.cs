using DryIoc;
using Microsoft.UI.Xaml;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Args;
using ShadowPluginLoader.WinUI.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            var session = Loader.StartScan();
            var ids = await session
                .Scan(new DirectoryInfo(Loader.PluginFolderPath))
                .FinishAsync();
            Task.Delay(1500).ContinueWith(_ => { Loader.Load(ids); });
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }

        private async void InstallButton_OnClick(object sender, RoutedEventArgs e)
        {
            var path =
                @"C:\Users\Kit_U\source\repos\kitUIN\ShadowPluginLoader.WinUI\package\ShadowExample.Plugin.Emoji-1.0.2.6-Debug.sdow";
            await Loader.InstallAsync([path]);
        }

        private async void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pluginId = "Emoji";
            await Loader.RemovePlugin(pluginId);
            RemoveButton.Content = "等待重启";
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
            var pluginId = "Emoji";
            var path =
                @"C:\Users\Kit_U\source\repos\kitUIN\ShadowPluginLoader.WinUI\package\ShadowExample.Plugin.Emoji-1.0.2.7-Debug.sdow";
            await Loader.UpgradePlugin(pluginId, new Uri(path));
            UpgradeButton.Content = "等待重启";
            RebootButton.IsEnabled = true;
        }
    }
}