using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
public class SettingsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("SettingsGenerator", context);
        if (context.SyntaxReceiver is not EnumSyntaxReceiver receiver)
        {
            logger.Warning("SPLW002", "No Setting Enum file found, skip Settings generation.");
            return;
        }

        foreach (var enumDeclaration in receiver.Enums)
        {
            var model = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);

            if (model.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
                continue;
            var namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();
            var enumClassName = enumSymbol.Name;
            const string attributeName = "ShadowPluginLoader.MetaAttributes.ShadowSettingAttribute";
            const string attributeClassName = "ShadowPluginLoader.MetaAttributes.ShadowPluginSettingClassAttribute";
            var keys = new List<string>();
            var inits = new List<string>();
            foreach (var member in enumSymbol.GetMembers().OfType<IFieldSymbol>())
            {
                if (!member.HasAttribute(context, attributeName)) continue;
                var comment = member.GetAttributeConstructorArgument<string?>(context, attributeName, 2);
                var defaultVal = member.GetAttributeConstructorArgument<string?>(context, attributeName, 1);
                var typeFullName = member.GetAttributeConstructorArgument<string>(context, attributeName, 0);
                if (defaultVal != null)
                {
                    inits.Add($$"""
                                        if(!SettingsHelper.Contains(Container, "{{member.Name}}"))
                                        {
                                            {{member.Name}} = {{defaultVal}};
                                        }
                                """);
                }

                keys.Add($$"""
                               /// <summary>
                               /// {{comment ?? ""}}
                               /// </summary>
                               public static {{typeFullName}} {{member.Name}}
                               {
                                   get => SettingsHelper.Get<{{typeFullName}}>(Container,"{{member.Name}}")!;
                                   set => SettingsHelper.Set(Container,"{{member.Name}}",value);
                               }
                           """);
            }

            if (keys.Count == 0) continue;
            var reswEnumCode = $$"""
                                 // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                 using ShadowPluginLoader.WinUI.Helpers;

                                 namespace {{namespaceName}};

                                 public partial class Settings
                                 {
                                     const string Container = "{{namespaceName}}.{{enumClassName}}";
                                    
                                 {{string.Join("\n", keys)}}
                                    
                                     public Settings()
                                     {
                                         BeforeInit();
                                         Init();
                                         AfterInit();
                                     }
                                     private void Init()
                                     {
                                 {{string.Join("\n", inits)}}
                                     }
                                     partial void BeforeInit();
                                      
                                     partial void AfterInit();
                                 }

                                 """;
            context.AddSource($"Settings.g.cs", reswEnumCode);
            if (!enumSymbol.HasAttribute(context, attributeClassName))
            {
                logger.Warning("SPLW003", $"No {attributeClassName} found on {enumClassName}, skip Plugin Settings generation.");
                continue;
            }

            var pluginType = enumSymbol.GetAttributeConstructorArgument<string>(context, attributeClassName, 0);
            var pluginNamespace = enumSymbol.GetAttributeConstructorArgument<string>(context, attributeClassName, 1);

            var code = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator

                         namespace {{pluginNamespace}};
                         
                         public partial class {{pluginType}}
                         {
                             /// <summary>
                             /// Settings
                             /// </summary>
                             public static {{namespaceName}}.Settings Settings { get; } = new {{namespaceName}}.Settings();
                         }
                         
                         """;
            context.AddSource($"{pluginType}_Settings.g.cs", code);
        }
    }
}

public class EnumSyntaxReceiver : ISyntaxReceiver
{
    public List<EnumDeclarationSyntax> Enums { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is EnumDeclarationSyntax enumDeclaration)
        {
            Enums.Add(enumDeclaration);
        }
    }
}