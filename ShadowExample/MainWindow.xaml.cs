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
            await loader.ImportFromDirAsync(@"D:\VsProjects\WASDK\ShadowPluginLoader.WinUI\ShadowExample.Plugin.Emoji\bin\Debug\net6.0-windows10.0.19041.0");
            foreach (var plugin in loader.GetPlugins())
            {
                Debug.WriteLine(plugin.GetEmoji());
                MyPanel.Children.Add(plugin.GetControl());
            }
        }
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}
