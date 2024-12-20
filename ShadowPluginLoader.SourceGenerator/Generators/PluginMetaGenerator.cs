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

    private static string GetValue(JToken token)
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

        var logger = new Logger("PluginMetaGenerator", context);
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
                    // var props = _pluginDNode!.Value<JObject>("Properties")!;
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
                    var code = $$"""
                                 // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                 using {{np}};

                                 namespace {{classSymbol.ContainingNamespace.ToDisplayString()}}
                                 {
                                     [{{meta}}]
                                     public partial class {{classSymbol.Name}}
                                     {
                                         /// <inheritdoc/>
                                         public override string Id => {{pluginId}};
                                 
                                         /// <summary>
                                         /// PluginMetaData
                                         /// </summary>
                                         public static {{metaType}} Meta { get; } = new {{metaType}}
                                         {
                                             {{meta2}}
                                         };
                                     }
                                 }
                                 """;
                    // Add the generated code to the compilation
                    context.AddSource($"{classSymbol.Name}.g.cs", code);
                    
                }
            }
        }
        catch (Exception e)
        {

            logger.Error("SPLE000", e.Message);
            throw e;
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}