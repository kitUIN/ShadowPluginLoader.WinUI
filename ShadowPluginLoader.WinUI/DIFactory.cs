using System;
using DryIoc;
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
        Services.Register<IPluginInstaller, ZipPluginInstaller>(reuse: Reuse.Singleton);
    }
}