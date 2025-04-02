using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    private Dictionary<string, List<string>> EntryPoints { get; } = new();

    private void GetEntryPoints(GeneratorExecutionContext context, PluginMetaSyntaxReceiver receiver, Logger logger)
    {
        var compilation = context.Compilation;
        var entryPointAttributeSymbol =
            compilation.GetTypeByMetadataName("ShadowPluginLoader.Attributes.EntryPointAttribute");

        if (entryPointAttributeSymbol == null) return;

        foreach (var classDeclaration in receiver.CandidateClasses)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol1 = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
            if (classSymbol1 == null) continue;
            foreach (var attributeData in classSymbol1.GetAttributes())
            {
                var attributeClass = attributeData.AttributeClass;
                if (attributeClass == null ||
                    (!attributeClass.Equals(entryPointAttributeSymbol, SymbolEqualityComparer.Default) &&
                     !InheritsFrom(attributeClass, entryPointAttributeSymbol))) continue;
                var name = GetNameValue(attributeData);
                if (name == null) continue;
                if (!EntryPoints.ContainsKey(name)) EntryPoints.Add(name, []);
                EntryPoints[name].Add(classSymbol1.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
        }
    }


    private string GetValue(JObject dNode, JToken? pluginNode,
        bool pluginTokenIsObject, int depth = 0)
    {
        if (pluginNode == null) return "null";
        if (!pluginTokenIsObject)
            return pluginNode.Type switch
            {
                JTokenType.Boolean => pluginNode.Value<bool>().ToString().ToLower(),
                JTokenType.String => $"\"{pluginNode.Value<string>()}\"",
                JTokenType.Array => "[" + string.Join(",",
                    pluginNode.Values().Select(x => GetValue(dNode, x, pluginTokenIsObject, depth + 1)).ToList()) + "]",
                _ => $"{pluginNode}",
            };
        var attrs = new List<string>();
        foreach (var item in dNode.Value<JObject>("Properties")!)
        {
            var dObj = item.Value!.Value<JObject>()!;

            var name = dObj.Value<string>("PropertyGroupName");
            var entryPointName = dObj.Value<string>("EntryPointName");
            if (entryPointName != null && EntryPoints.ContainsKey(entryPointName) &&
                EntryPoints[entryPointName].Count > 0)
            {
                if (dNode.ContainsKey("Item"))
                    attrs.Add($"{name} = [" +
                              string.Join(",", EntryPoints[entryPointName].Select(x => $"typeof({x})")) + "]");
                else attrs.Add($"{name} = typeof({EntryPoints[entryPointName][0]})");
                continue;
            }

            var pluginObj = pluginNode.Value<JObject>()!;
            if (name == null) continue;
            var pluginValue = pluginObj.ContainsKey(name) ? pluginObj.GetValue(name) : new JObject();
            var token = GetValue(dObj, pluginValue, dObj.ContainsKey("Properties"), depth + 1);
            if (token == "{}") continue;
            attrs.Add($"{name} = {token}");
            if (name == "Id" && _pluginId == "") _pluginId = token;
        }

        var indent = "\n\t\t\t" + new string('\t', depth + 1);
        var resultIndent = "\n\t\t\t" + new string('\t', depth);
        if (_buildIn && depth == 0)
        {
            attrs.Add($"BuiltIn = {_buildIn.ToString().ToLower()}");
        }

        var result = string.Join(",", attrs.Select(attr => indent + attr));
        var newType = dNode.Value<string>("Type")!;
        return $"new {newType}{resultIndent}{{{result}{resultIndent}}}";
    }


    private string _pluginId = "";

    private static bool InheritsFrom(INamedTypeSymbol symbol, INamedTypeSymbol baseType)
    {
        // 遍历基类链，检查是否匹配
        while (symbol.BaseType != null)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.BaseType, baseType))
                return true;

            symbol = symbol.BaseType;
        }

        return false;
    }

    private string? GetNameValue(AttributeData attributeData)
    {
        // 1. 首先检查具名参数 (用于 EntryPointAttribute)
        var nameArgument = attributeData.NamedArguments
            .FirstOrDefault(kv => kv.Key == "Name").Value;

        if (nameArgument.Value is string namedValue && !string.IsNullOrEmpty(namedValue))
        {
            return namedValue; // 直接返回具名参数值
        }

        return null;
    }

    private bool _buildIn;

    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("PluginMetaGenerator", context);
        try
        {
            if (context.SyntaxReceiver is not PluginMetaSyntaxReceiver receiver) return;
            GetEntryPoints(context, receiver, logger);
            if (receiver.Plugin == null) return;
            var model = context.Compilation.GetSemanticModel(receiver.Plugin.SyntaxTree);

            if (model.GetDeclaredSymbol(receiver.Plugin) is not INamedTypeSymbol classSymbol)
                return;

            if (!classSymbol.HasAttribute(context,
                    "ShadowPluginLoader.Attributes.MainPluginAttribute")) return;
            var mainAtrr = classSymbol.GetAttribute(context, "ShadowPluginLoader.Attributes.MainPluginAttribute")
            var nameArgument = mainAtrr?.NamedArguments
                .FirstOrDefault(kv => kv.Key == "BuiltIn").Value;

            if (nameArgument?.Value is bool namedValue)
            {
                _buildIn = namedValue;
            }

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
                                 
                                 /// <inheritdoc/>
                                 public override {{metaType}} MetaData => Meta;
                                 
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