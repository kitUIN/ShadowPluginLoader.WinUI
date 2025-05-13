using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using ShadowExample.Plugin.Emoji;
using System;
using DryIoc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ShadowExample.Core;
using ShadowPluginLoader.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class UnitTestAppWindow : Window
    {
        public UnitTestAppWindow()
        {
            this.InitializeComponent();
        }

        private async void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var _loader = DiFactory.Services.Resolve<ShadowExamplePluginLoader>();
                
                
            }
            catch (Exception ex)
            {
                throw; // TODO handle exception
            }
        }
    }
}