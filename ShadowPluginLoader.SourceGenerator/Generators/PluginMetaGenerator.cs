using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
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

    private string GetValue(JObject dNode, JToken? pluginNode, bool pluginTokenIsObject)
    {
        if (pluginNode == null) return "null";
        if (!pluginTokenIsObject)
            return pluginNode.Type switch
            {
                JTokenType.Boolean => pluginNode.Value<bool>().ToString().ToLower(),
                JTokenType.String => $"\"{pluginNode.Value<string>()}\"",
                JTokenType.Array => "[" + string.Join(",",
                    pluginNode.Values().Select(x => GetValue(dNode, x, pluginTokenIsObject)).ToList()) + "]",
                _ => $"{pluginNode}",
            };
        var attrs = new List<string>();
        foreach (var item in dNode.Value<JObject>("Properties")!)
        {
            var dObj = item.Value!.Value<JObject>()!;
            var pluginObj = pluginNode.Value<JObject>()!;
            var name = dObj.Value<string>("PropertyGroupName");
            if (name == null || !pluginObj.ContainsKey(name)) continue;
            var pluginValue = pluginObj.GetValue(name);
            var token = GetValue(dObj, pluginValue, dObj.ContainsKey("Properties"));
            attrs.Add($"{name} = {token}");
            if (name == "Id" && _pluginId == "") _pluginId = token;
        }

        var meta2 = string.Join(",\n            ", attrs);
        var newType = dNode.Value<string>("Type")!;
        return $"new {newType} {{\n            {meta2} }}";
    }

    private string _pluginId = "";

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
            var metaType = _pluginDNode!.Value<string>("Type");
            var dNode = _pluginDNode;
            var pluginNode = _pluginNode;
            var meta2 = GetValue(dNode, pluginNode, true);
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
                                 public override string Id => {{_pluginId}};
                         
                                 /// <summary>
                                 /// PluginMetaData
                                 /// </summary>
                                 public static {{metaType}} Meta { get; } = {{meta2}};
                             }
                         }
                         """;
            context.AddSource($"{classSymbol.Name}.g.cs", code);
        }
        catch (Exception e)
        {
            logger.Error("SPLE000", $"{e}");
            throw e;
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new PluginMetaSyntaxReceiver());
    }
}