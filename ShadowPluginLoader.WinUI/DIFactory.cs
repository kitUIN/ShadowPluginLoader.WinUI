using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Checkers;
using ShadowPluginLoader.WinUI.Config;
using ShadowPluginLoader.WinUI.Helpers;
using ShadowPluginLoader.WinUI.PluginFactories;
using ShadowPluginLoader.WinUI.Processors;
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
        Services.Register<IDependencyChecker<TMeta>, DependencyChecker<TMeta>>(reuse: Reuse.Singleton,
            ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        Services.Register<IRemoveChecker, RemoveChecker>(reuse: Reuse.Singleton,
            ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        Services.Register<IUpgradeChecker, UpgradeChecker>(reuse: Reuse.Singleton,
            ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        Services.Register<IMainProcessor, MainProcessor<TAPlugin, TMeta>>(Reuse.Singleton,
            ifAlreadyRegistered: IfAlreadyRegistered.Replace);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RegisterPluginLoader<T>() where T : IPluginFactory
    {
        Services.Register<T>(Reuse.Singleton);
        Services.Register<IPluginFactory, T>(Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
    }
}