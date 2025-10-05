using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.Installer;
using ShadowPluginLoader.WinUI.Scanners;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// Dependency injection factory
/// </summary>
public static class DiFactory
{
    /// <summary>
    /// Dependency injection container
    /// </summary>
    public static Container Services { get; }

    static DiFactory()
    {
        Services = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments));
        Services.Register(
            Made.Of(() => Serilog.Log.ForContext(Arg.Index<Type>(0)),
                r => r.ImplementationType ?? r.Parent.ImplementationType ?? typeof(object)),
            setup: Setup.With(condition: r => r.Parent.ImplementationType != null || r.ImplementationType != null));
        Services.Register<PluginEventService>(reuse: Reuse.Singleton);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Init<TAPlugin, TMeta>()
        where TAPlugin : AbstractPlugin<TMeta>
        where TMeta : AbstractPluginMetaData
    {
        MetaDataHelper.Init<TMeta>();
        var baseSdkConfig = BaseSdkConfig.Load();
        Services.RegisterInstance(baseSdkConfig);
        var innerSdkConfig = InnerSdkConfig.Load();
        Services.RegisterInstance(innerSdkConfig);
        Services.Register<IDependencyChecker<TMeta>, DependencyChecker<TMeta>>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IRemoveChecker, RemoveChecker>(serviceKey: "base",
            reuse: Reuse.Singleton);
        Services.Register<IPluginScanner<TAPlugin, TMeta>, PluginScanner<TAPlugin, TMeta>>(
            serviceKey: "base", reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
            ); 
        Services.Register<IPluginInstaller<TMeta>, ZipPluginInstaller<TMeta>>(
            serviceKey: "base",  reuse: Reuse.Singleton, 
            made: Parameters.Of
                .Type<IDependencyChecker<TMeta>>(serviceKey: "base")
                .OverrideWith(Parameters.Of.Type<IPluginScanner<TAPlugin, TMeta>>(serviceKey: "base"))
            );
    }
}