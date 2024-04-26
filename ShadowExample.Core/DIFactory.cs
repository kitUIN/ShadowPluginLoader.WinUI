using System;
using DryIoc;
using ShadowExample.Core.Plugins;
using ShadowPluginLoader.WinUI;

namespace ShadowExample.Core;

public static class DiFactory
{
    public static Container Services { get; }
    static DiFactory()
    {
        Services = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments));
        Services.Register(
            Made.Of(() => Serilog.Log.ForContext(Arg.Index<Type>(0)), r => r.Parent.ImplementationType),
            setup: Setup.With(condition: r => r.Parent.ImplementationType != null));
        APluginLoader<ExampleMetaData, AExamplePlugin>.Services = Services;
        Services.Register<ShadowExamplePluginLoader>(reuse: Reuse.Singleton);
    }

}