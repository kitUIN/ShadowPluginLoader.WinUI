using Microsoft.CodeAnalysis;
using ShadowPluginLoader.SourceGenerator.Receivers;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
public class SettingsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SettingsSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("SettingsGenerator", context);
        if (context.SyntaxReceiver is not SettingsSyntaxReceiver receiver)
        {
            logger.Warning("SPLW002", "No Setting Enum file found, skip Settings generation.");
            return;
        }

        try
        {
            var globalNamespace = context.Compilation.GlobalNamespace;
            var topLevelNamespace = globalNamespace.GetNamespaceMembers().FirstOrDefault()!.ToDisplayString();
            foreach (var enumDeclaration in receiver.Enums)
            {
                var model = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);

                if (model.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
                    continue;

                var namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();
                const string attributeName = "ShadowPluginLoader.Attributes.ShadowSettingAttribute";
                const string attributeClassAliasName =
                    "ShadowPluginLoader.Attributes.ShadowSettingClassAttribute";
                var keys = new List<string>();
                var inits = new List<string>();
                foreach (var member in enumSymbol.GetMembers().OfType<IFieldSymbol>())
                {
                    if (member == null) continue;
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
                var accessorClassName = "Settings";
                if (keys.Count == 0) continue;
                var container = topLevelNamespace;
                var attributeClass = enumSymbol.GetAttribute(context, attributeClassAliasName);
                if (attributeClass != null)
                {
                    foreach (var namedArgument in attributeClass.NamedArguments)
                    {
                        var name = namedArgument.Key;
                        if (namedArgument.Value.Value == null) continue;
                        switch (name)
                        {
                            case "Container":
                                container = (string)namedArgument.Value.Value;
                                break;
                            case "ClassName":
                                settingsClassName = (string)namedArgument.Value.Value;
                                break;
                            case "PluginAccessorName":
                                accessorClassName = (string)namedArgument.Value.Value;
                                break;
                        }
                    }
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
                                         const string Container = "{{container}}";
                                             
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

                if (receiver.Plugin == null) continue;
                var pluginModel = context.Compilation.GetSemanticModel(receiver.Plugin.SyntaxTree);
                if (pluginModel.GetDeclaredSymbol(receiver.Plugin) is not INamedTypeSymbol pluginSymbol)
                    continue;
                var pluginName = pluginSymbol.Name;
                var pluginNamespace = pluginSymbol.ContainingNamespace.ToDisplayString();
                var code = $$"""
                             // Automatic Generate From ShadowPluginLoader.SourceGenerator

                             namespace {{pluginNamespace}};

                             /// <summary>
                             /// 
                             /// </summary>
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
        catch (Exception ex)
        {
            logger.Error("PL000", $"{ex.Message},{ex.StackTrace}");
        }
    }
}