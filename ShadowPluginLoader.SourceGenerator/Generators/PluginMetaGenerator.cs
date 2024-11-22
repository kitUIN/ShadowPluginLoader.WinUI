using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
internal class PluginMetaGenerator : ISourceGenerator
{
    private static JObject? _pluginNode;
    private static JObject? _pluginDNode;

    private static void GetJson(GeneratorExecutionContext context)
    {
        foreach (var file in context.AdditionalFiles)
        {
            var name = Path.GetFileName(file.Path);
            if (name.Equals("plugin.json", StringComparison.OrdinalIgnoreCase))
            {
                var jsonString = file.GetText();
                if (jsonString is not null)
                {
                    _pluginNode = JObject.Parse(jsonString.ToString());
                    if (_pluginNode is null)
                    {
                        throw new Exception("plugin.json Is Empty");
                    }
                }
                else
                {
                    throw new Exception("Not Found plugin.json");
                }
            }
            else if (name.Equals("plugin.d.json", StringComparison.OrdinalIgnoreCase))
            {
                var jsonString = file.GetText();
                if (jsonString is not null)
                {
                    _pluginDNode = JObject.Parse(jsonString.ToString());
                    if (_pluginDNode is null)
                    {
                        throw new Exception("plugin.d.json Is Empty");
                    }
                }
                else
                {
                    throw new Exception("Not Found plugin.d.json");
                }
            }
        }
    }

    public string GetValue(JToken token)
    {

        return token.Type switch
        {
            JTokenType.String => $"\"{token.Value<string>()}\"",
            JTokenType.Array => "[" + string.Join(",", token.Values().Select(GetValue).ToList()) + "]",
            _ => $"{token}",
        };
    }
    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            // Get the compilation object
            var compilation = context.Compilation;

            // Get the symbol for the Serializable attribute
            var serializableSymbol =
                compilation.GetTypeByMetadataName("ShadowPluginLoader.MetaAttributes.AutoPluginMetaAttribute");
            
            GetJson(context);
            // Loop through the syntax trees in the compilation
            foreach (var tree in compilation.SyntaxTrees)
            {
                // Get the semantic model for the syntax tree
                var model = compilation.GetSemanticModel(tree);

                // Find all the class declarations in the syntax tree
                var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
                // Loop through the class declarations
                
                foreach (var cls in classes)
                {
                    // Get the symbol for the class declaration
                    var classSymbol = model.GetDeclaredSymbol(cls);

                    // Check if the class has the Serializable attribute
                    if (!classSymbol!.GetAttributes().Any(a =>
                            a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default))) continue;
                    var np = _pluginDNode!.Value<string>("Namespace");
                    var metaType = _pluginDNode!.Value<string>("Type");
                    var attrs = new List<string>();
                    var pluginId = "";
                    foreach (var attr in _pluginNode!)
                    {
                        if (attr.Key == "Id") pluginId = GetValue(attr.Value!);
                        attrs.Add($"{attr.Key} = {GetValue(attr.Value!)}");
                    } 
                    var meta = $"{metaType}({string.Join(", ", attrs)})";
                    var meta2 = string.Join(",\n            ", attrs);
                    // Generate a Hello method with the class name
                    var code = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using {np};

namespace {classSymbol.ContainingNamespace.ToDisplayString()}
{{
    [{meta}]
    public partial class {classSymbol.Name}
    {{
        /// <inheritdoc/>
        public override string Id => {pluginId};

        /// <summary>
        /// PluginMetaData
        /// </summary>
        public static {metaType} Meta {{ get; }} = new {metaType}
        {{
            {meta2}
        }};
    }}
}}";
                    // Add the generated code to the compilation

                    context.AddSource($"{classSymbol.Name}.g.cs", code);
                    context.AddSource($"PluginPathHelper.g.cs",$@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using CustomExtensions.WinUI;
using {classSymbol.ContainingNamespace.ToDisplayString()};

namespace {classSymbol.ContainingNamespace.ToDisplayString()}.Helpers;

/// <summary>
/// Provides utility methods for handling plugin paths and URIs.
/// </summary>
internal static class PluginPathHelper
{{
    /// <summary>
    /// Converts the given path to a local file path format for plugins.
    /// If the path starts with ""ms-appx:///"", it is converted to a local path format.
    /// </summary>
    /// <param name=""path"">The plugin path, which can start with ""ms-appx:///"".</param>
    /// <returns>A string representing the local file path.</returns>
    public static string PluginPath(string path) 
    {{
        if(path.StartsWith(""ms-appx:///""))
        {{
            return path.Replace(""ms-appx:///"",""/"").AssetPath<{classSymbol.Name}>();
        }}
        return path.AssetPath<{classSymbol.Name}>();
    }}

    /// <summary>
    /// Converts the given path to a URI object.
    /// </summary>
    /// <param name=""path"">The plugin path, which can start with ""ms-appx:///"".</param>
    /// <returns>A <see cref=""Uri""/> object representing the path.</returns>
    public static string PluginUri(string path) 
    {{
        return new Uri(PluginPath(path));
    }}
}}
");
                }
            }
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