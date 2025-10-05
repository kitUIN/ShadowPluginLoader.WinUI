using CustomExtensions.WinUI;
using DryIoc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using ShadowExample.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ShadowExample.Core.Plugins;
using ShadowObservableConfig.Yaml;
using ShadowPluginLoader.WinUI;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Installer;
using ShadowPluginLoader.WinUI.Scanners;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShadowExample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            ApplicationExtensionHost.Initialize(this);
            ShadowObservableConfig.ShadowConfigGlobalSetting.Init(new ShadowYamlConfigSetting());
            Init();
        }

        private void Init()
        {
            DiFactory.Init<PluginBase, ExampleMetaData>();
            DiFactory.Services.Register<ShadowExamplePluginLoader>(
                reuse: Reuse.Singleton,
                made: Parameters.Of
                    .Type<IDependencyChecker<ExampleMetaData>>(serviceKey: "base")
                    .OverrideWith(Parameters.Of.Type<IRemoveChecker>(serviceKey: "base"))
                    .OverrideWith(Parameters.Of.Type<IPluginScanner<PluginBase, ExampleMetaData>>(serviceKey: "base"))
                    .OverrideWith(Parameters.Of.Type<IPluginInstaller<ExampleMetaData>>(serviceKey: "base"))
                );
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
