using DryIoc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ShadowExample.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
            Init();
        }
        private async void Init()
        {
            var loader = DiFactory.Services.Resolve<ShadowExamplePluginLoader>();
            var logger = Log.ForContext<MainWindow>();
            
            // 使用两阶段加载模式
            await TwoPhaseLoadingExample(loader, logger);
        }

        /// <summary>
        /// 两阶段加载示例
        /// </summary>
        private async Task TwoPhaseLoadingExample(ShadowExamplePluginLoader loader, ILogger logger)
        {
            logger.Information("=== 开始两阶段插件加载 ===");
            
            // 阶段1：扫描插件
            loader.Scan(typeof(ShadowExample.Plugin.Emoji.EmojiPlugin));
            loader.Scan(typeof(ShadowExample.Plugin.Hello.HelloPlugin));
            
            // 阶段2：预加载DLL（快速启动）
            var preloadStopwatch = System.Diagnostics.Stopwatch.StartNew();
            await loader.CheckUpgradeAndRemoveAsync();
            var preloadedPlugins = await loader.PreloadAsync();
            preloadStopwatch.Stop();
            
            logger.Information("DLL预加载完成，耗时: {ms}ms，预加载插件数量: {count}", 
                preloadStopwatch.ElapsedMilliseconds, preloadedPlugins.Count);
            
            // 主程序UI初始化完成后的延迟实例化
            await Task.Delay(1000); // 模拟主程序初始化时间
            
            // 阶段3：延迟实例化插件
            var instantiateStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var instantiatedCount = await loader.InstantiatePluginsAsync();
            instantiateStopwatch.Stop();
            
            logger.Information("插件实例化完成，耗时: {ms}ms，实例化插件数量: {count}", 
                instantiateStopwatch.ElapsedMilliseconds, instantiatedCount);
            
            // 显示插件UI
            foreach (var plugin in loader.GetPlugins())
            {
                Debug.WriteLine($"插件: {plugin.DisplayName}");
                // 注意：这里需要根据实际插件类型调用相应方法
                // MyPanel.Children.Add(plugin.GetControl());
            }
        }
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}
