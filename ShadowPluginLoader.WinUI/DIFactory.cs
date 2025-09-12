using System;
using DryIoc;
using ShadowPluginLoader.WinUI.Scanners;
using ShadowPluginLoader.WinUI.Services;

namespace ShadowPluginLoader.WinUI;

/// <summary>
/// 依赖注入-工厂
/// </summary>
public static class DiFactory
{
    /// <summary>
    /// 依赖注入-容器
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
        Services.Register<PluginYamlConfigService>(Reuse.Singleton);
        Services.Register<IPluginInstaller, ZipPluginInstaller>(reuse: Reuse.Singleton);
    }
}