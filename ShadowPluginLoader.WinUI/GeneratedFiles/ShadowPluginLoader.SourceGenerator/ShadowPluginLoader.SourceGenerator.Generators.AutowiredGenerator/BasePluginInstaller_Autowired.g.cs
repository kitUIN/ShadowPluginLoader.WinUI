// Automatic Generate From ShadowPluginLoader.SourceGenerator

namespace ShadowPluginLoader.WinUI.Services;

public partial class BasePluginInstaller
{
    /// <summary>
    /// 
    /// </summary>
    public BasePluginInstaller(global::Serilog.ILogger logger)
    {
       Logger = logger;
       ConstructorInit();
    }
    
    /// <summary>
    /// Constructor Init
    /// </summary>
    partial void ConstructorInit();
}