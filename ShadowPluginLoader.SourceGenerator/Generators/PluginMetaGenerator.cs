using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using ShadowPluginLoader.SourceGenerator.Receivers;

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
            JTokenType.Boolean => token.Value<bool>().ToString().ToLower(),
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
            if (context.SyntaxReceiver is not PluginMetaSyntaxReceiver receiver) return;
            if (receiver.Plugin == null) return;

            var model = context.Compilation.GetSemanticModel(receiver.Plugin.SyntaxTree);

            if (model.GetDeclaredSymbol(receiver.Plugin) is not INamedTypeSymbol classSymbol)
                return;

            if (!classSymbol.HasAttribute(context,
                    "ShadowPluginLoader.Attributes.MainPluginAttribute")) return;
            GetJson(context);
            var metaType = _pluginDNode!.Value<string>("MetaDataType");
            var attrs = new List<string>();
            var pluginId = "";
            foreach (var attr in _pluginNode!)
            {
                if (attr.Key == "Id") pluginId = GetValue(attr.Value!);
                attrs.Add($"{attr.Key} = {GetValue(attr.Value!)}");
            }

            var meta2 = string.Join(",\n            ", attrs);
            var code = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator


                         namespace {{classSymbol.ContainingNamespace.ToDisplayString()}}
                         {
                             /// <summary>
                             /// 
                             /// </summary>
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
            context.AddSource($"{classSymbol.Name}.g.cs", code);
        }
        catch (Exception e)
        {
            logger.Error("SPLE000", e.Message);
            throw e;
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new PluginMetaSyntaxReceiver());
    }
}