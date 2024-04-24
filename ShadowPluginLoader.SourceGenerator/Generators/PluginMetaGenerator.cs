using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
internal class PluginMetaGenerator : ISourceGenerator
{
    private static JObject? PluginNode;
    private static JObject? PluginDNode;
    static void GetJson(GeneratorExecutionContext context)
    {
        foreach (var file in context.AdditionalFiles)
        {
            var name = Path.GetFileName(file.Path);
            if (name.Equals("plugin.json", StringComparison.OrdinalIgnoreCase))
            {
                var jsonString = file.GetText();
                if (jsonString is null)
                {
                    throw new Exception($"Not Found plugin.json");
                }
                PluginNode = JObject.Parse(jsonString.ToString());
                if(PluginNode is null)
                {
                    throw new Exception($"plugin.json Is Empty");
                }
            }
            else if (name.Equals("plugin.d.json", StringComparison.OrdinalIgnoreCase))
            {
                var jsonString = file.GetText();
                if (jsonString is null)
                {
                    throw new Exception($"Not Found plugin.d.json");
                }
                PluginDNode = JObject.Parse(jsonString.ToString());
                if(PluginDNode is null)
                {
                    throw new Exception($"plugin.d.json Is Empty");
                }
            }
        }
    }
    public string GetValue(JToken token)
    {
        return token.Type switch
        {
            JTokenType.String => $"\"{token.Value<string>()}\"",
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
                compilation.GetTypeByMetadataName("ShadowPluginLoader.SourceGenerator.Attributes.AutoPluginMetaAttribute");
            
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
                    if (classSymbol!.GetAttributes().Any(a =>
                            a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default)))
                    {
                        
                        var np = PluginDNode!.Value<string>("Namespace");
                        var metaType = PluginDNode!.Value<string>("Type");
                        var attrs = new List<string>();
                        foreach (var attr in PluginNode!)
                        {
                            if(attr.Value!.Type == JTokenType.Array)
                            {
                                if (attr.Value.Count() == 0) continue;
                                var ll = new List<string>();
                                foreach(var attr2 in attr.Value.Values())
                                {
                                    ll.Add(GetValue(attr2));
                                }
                                attrs.Add(attr.Key + "= new [] { " + string.Join(",", ll) + "}");
                            }
                            else
                            {
                                attrs.Add($"{attr.Key} = {GetValue(attr.Value)}");
                            }
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
                    }
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