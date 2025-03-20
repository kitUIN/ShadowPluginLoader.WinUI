using Microsoft.CodeAnalysis;
using ShadowPluginLoader.SourceGenerator.Receivers;

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

        var globalNamespace = context.Compilation.GlobalNamespace;
        var topLevelNamespace = globalNamespace.GetNamespaceMembers().FirstOrDefault()!.ToDisplayString();
        foreach (var enumDeclaration in receiver.Enums)
        {
            var model = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);

            if (model.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
                continue;

            var namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();
            var enumClassName = enumSymbol.Name;
            const string attributeName = "ShadowPluginLoader.MetaAttributes.ShadowSettingAttribute";
            const string attributeClassName = "ShadowPluginLoader.MetaAttributes.ShadowPluginSettingClassAttribute";
            const string attributeClassAliasName =
                "ShadowPluginLoader.MetaAttributes.ShadowSettingClassAttribute";
            var keys = new List<string>();
            var inits = new List<string>();
            foreach (var member in enumSymbol.GetMembers().OfType<IFieldSymbol>())
            {
                if (!member.HasAttribute(context, attributeName)) continue;
                var typeSettingSymbol =
                    member.GetAttributeConstructorArgument<INamedTypeSymbol>(context, attributeName, 0);
                var typeFullName = typeSettingSymbol.Name;
                var typeSettingNamespace = typeSettingSymbol.ContainingNamespace.ToDisplayString();
                typeFullName = typeSettingNamespace + "." + typeFullName;
                var defaultVal = member.GetAttributeConstructorArgument<string?>(context, attributeName, 1);
                var comment = member.GetAttributeConstructorArgument<string?>(context, attributeName, 2);
                var isPath = member.GetAttributeConstructorArgument<bool>(context, attributeName, 3);
                var baseFolder = member.GetAttributeConstructorArgument<int>(context, attributeName, 4);
                var baseFolderName = baseFolder switch
                {
                    0 => "Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path",
                    1 => "Windows.Storage.ApplicationData.Current.LocalFolder.Path",
                    2 => "Windows.Storage.ApplicationData.Current.RoamingFolder.Path",
                    3 => "Windows.Storage.ApplicationData.Current.SharedLocalFolder.Path",
                    4 => "Windows.Storage.ApplicationData.Current.TemporaryFolder.Path",
                    _ => ""
                };

                if (defaultVal != null)
                {
                    if (typeFullName == "System.String") defaultVal = "\"" + defaultVal + "\"";
                    
                    inits.Add(isPath
                        ? $$"""
                                    if(!SettingsHelper.Contains(Container, "{{member.Name}}"))
                                    {
                                        {{member.Name}} = System.IO.Path.Combine({{baseFolderName}}, {{defaultVal}});
                                    }
                                    if (!System.IO.Directory.Exists({{member.Name}}))
                                    {
                                        System.IO.Directory.CreateDirectory({{member.Name}});
                                    }
                            """
                        : $$"""
                                    if(!SettingsHelper.Contains(Container, "{{member.Name}}"))
                                    {
                                        {{member.Name}} = {{defaultVal}};
                                    }
                            """);
                }
                else if (isPath)
                {
                    inits.Add($$"""
                                        if(!SettingsHelper.Contains(Container, "{{member.Name}}"))
                                        {
                                            {{member.Name}} = System.IO.Path.Combine({{baseFolderName}}, "{{member.Name}}");
                                        }
                                        if (!System.IO.Directory.Exists({{member.Name}}))
                                        {
                                            System.IO.Directory.CreateDirectory({{member.Name}});
                                        }
                                """);
                }

                keys.Add($$"""
                               /// <summary>
                               /// {{comment ?? ""}}
                               /// </summary>
                               public {{typeFullName}} {{member.Name}}
                               {
                                   get => SettingsHelper.Get<{{typeFullName}}>(Container, "{{member.Name}}")!;
                                   set => SettingsHelper.Set(Container, "{{member.Name}}", value);
                               }
                           """);
            }

            var settingsClassName = "Settings";
            if (keys.Count == 0) continue;
            if (enumSymbol.HasAttribute(context, attributeClassAliasName))
            {
                settingsClassName =
                    enumSymbol.GetAttributeConstructorArgument<string>(context, attributeClassAliasName, 1);
                topLevelNamespace =
                    enumSymbol.GetAttributeConstructorArgument<string>(context, attributeClassAliasName, 0);
            }

            var reswEnumCode = $$"""
                                 // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                 using ShadowPluginLoader.WinUI.Helpers;

                                 namespace {{namespaceName}};
                                 
                                 /// <summary>
                                 /// 
                                 /// </summary>
                                 public partial class {{settingsClassName}}
                                 {
                                 
                                     /// <summary>
                                     /// 
                                     /// </summary>
                                     const string Container = "{{topLevelNamespace}}";
                                         
                                 {{string.Join("\n", keys)}}
                                    
                                     /// <summary>
                                     /// 
                                     /// </summary>
                                     public {{settingsClassName}}()
                                     {
                                         BeforeInit();
                                         Init();
                                         AfterInit();
                                     }
                                     
                                     /// <summary>
                                     /// Init
                                     /// </summary>
                                     private void Init()
                                     {
                                             
                                 {{string.Join("\n", inits)}}
                                     }
                                     
                                     /// <summary>
                                     /// BeforeInit
                                     /// </summary>
                                     partial void BeforeInit();
                                      
                                     /// <summary>
                                     /// AfterInit
                                     /// </summary>
                                     partial void AfterInit();
                                 }

                                 """;
            context.AddSource($"{settingsClassName}.g.cs", reswEnumCode);
            if (!enumSymbol.HasAttribute(context, attributeClassName))
            {
                logger.Warning("SPLW003",
                    $"No {attributeClassName} found on {enumClassName}, skip Plugin Settings generation.");
                continue;
            }

            var typeSymbol =
                enumSymbol.GetAttributeConstructorArgument<INamedTypeSymbol>(context, attributeClassName, 0);
            var pluginName = typeSymbol.Name;
            var pluginNamespace = typeSymbol.ContainingNamespace.ToDisplayString();
            var accessorClassName = enumSymbol.GetAttributeConstructorArgument<string>(context, attributeClassName, 1);

            var code = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator

                         namespace {{pluginNamespace}};
                         
                         public partial class {{pluginName}}
                         {
                             /// <summary>
                             /// Settings
                             /// </summary>
                             public static {{namespaceName}}.{{settingsClassName}} {{accessorClassName}} { get; } = new {{namespaceName}}.{{settingsClassName}}();
                         }

                         """;
            context.AddSource($"{pluginName}_Settings.g.cs", code);
        }
    }
}

