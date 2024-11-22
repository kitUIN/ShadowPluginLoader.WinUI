using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
internal class PluginPathGenerator : ISourceGenerator
{
    
    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            var compilation = context.Compilation;
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.RootNamespace", out var currentNamespace);
            context.AddSource($"PluginImageSourceExtension.g.cs", $@"
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CustomExtensions.WinUI;

namespace {currentNamespace}.Extensions;

[MarkupExtensionReturnType(ReturnType = typeof(ImageSource))]
internal sealed class PluginImageSourceExtension : MarkupExtension
{{
	public string Source {{ get; set; }}

	protected override object ProvideValue()
	{{
		return new BitmapImage(new Uri(Source.PluginPath()));
	}}
}}");
            context.AddSource($"PluginPathExtension.g.cs", $@"
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CustomExtensions.WinUI;

namespace {currentNamespace}.Extensions;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal sealed class PluginPathExtension : MarkupExtension
{{
    public string Source {{ get; set; }}

	protected override object ProvideValue()
	{{
		return Source.PluginPath();
	}}
}}");
            context.AddSource($"PluginUriExtension.g.cs", $@"
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CustomExtensions.WinUI;

namespace {currentNamespace}.Extensions;

[MarkupExtensionReturnType(ReturnType = typeof(Uri))]
internal sealed class PluginUriExtension : MarkupExtension
{{
	public string Source {{ get; set; }}

	protected override object ProvideValue()
	{{
		return new Uri(Source.PluginPath());
	}}
}}");
        }
        catch (Exception e)
        {
            
            Logger.Error(context, e.Message);
            throw e;
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}