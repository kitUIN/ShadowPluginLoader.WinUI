using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowPluginLoader.SourceGenerator.Receivers;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
public class ConfigGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ConfigSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("ConfigGenerator", context);
        
        if (context.SyntaxReceiver is not ConfigSyntaxReceiver receiver)
        {
            logger.Warning("SPLW004", "No Config class found, skip Config generation.");
            return;
        }

        if (receiver.ConfigClasses.Count == 0)
        {
            logger.Warning("SPLW005", "No Config class found, skip Config generation.");
            return;
        }

        try
        {
            // 首先处理Config类，生成Config类代码
            foreach (var classDeclaration in receiver.ConfigClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
                
                if (classSymbol == null) continue;

                // 获取ConfigAttribute信息
                var configAttribute = classSymbol.GetAttribute(context, "ShadowPluginLoader.Attributes.ConfigAttribute");
                if (configAttribute == null) continue;

                // 提取ConfigAttribute参数
                var fileName = GetAttributeValue(configAttribute, "FileName", "config.json");
                var dirPath = GetAttributeValue(configAttribute, "DirPath", "config");
                var description = GetAttributeValue(configAttribute, "Description", "");
                var version = GetAttributeValue(configAttribute, "Version", "1.0.0");

                // 查找带有ConfigFieldAttribute的字段
                var configFields = GetConfigFields(classSymbol, context);

                if (configFields.Count == 0) continue;

                // 生成Config类代码
                var generatedCode = GenerateConfigClass(classSymbol, configFields, fileName, dirPath, description, version);
                
                var fileName_safe = classSymbol.Name.Replace("<", "").Replace(">", "");
                context.AddSource($"{fileName_safe}.Config.g.cs", generatedCode);
            }

            // 然后处理MainPlugin类，为每个MainPlugin类添加Config静态成员
            foreach (var mainPluginDeclaration in receiver.MainPluginClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(mainPluginDeclaration.SyntaxTree);
                var mainPluginSymbol = semanticModel.GetDeclaredSymbol(mainPluginDeclaration) as INamedTypeSymbol;
                
                if (mainPluginSymbol == null) continue;

                // 查找同命名空间下的Config类
                var configClass = FindConfigClassInSameNamespace(mainPluginSymbol, receiver.ConfigClasses, context);
                if (configClass == null) continue;

                // 生成MainPlugin类的扩展代码，添加Config静态成员
                var mainPluginExtensionCode = GenerateMainPluginConfigExtension(mainPluginSymbol, configClass);
                
                var mainPluginFileName_safe = mainPluginSymbol.Name.Replace("<", "").Replace(">", "");
                context.AddSource($"{mainPluginFileName_safe}.ConfigExtension.g.cs", mainPluginExtensionCode);
            }
        }
        catch (Exception ex)
        {
            logger.Error("SPLW006", $"ConfigGenerator error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private List<ConfigFieldInfo> GetConfigFields(INamedTypeSymbol classSymbol, GeneratorExecutionContext context)
    {
        var configFields = new List<ConfigFieldInfo>();
        const string configFieldAttributeName = "ShadowPluginLoader.Attributes.ConfigFieldAttribute";

        foreach (var member in classSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            if (!member.HasAttribute(context, configFieldAttributeName)) continue;

            var fieldAttribute = member.GetAttribute(context, configFieldAttributeName);
            if (fieldAttribute == null) continue;

            var fieldInfo = new ConfigFieldInfo
            {
                FieldName = member.Name,
                FieldType = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Name = GetAttributeValue(fieldAttribute, "Name", member.Name),
                Description = GetAttributeValue(fieldAttribute, "Description", ""),
                Alias = GetAttributeValue(fieldAttribute, "Alias", ""),
                ApplyNamingConventions = GetAttributeValue(fieldAttribute, "ApplyNamingConventions", true) == "True" || GetAttributeValue(fieldAttribute, "ApplyNamingConventions", true) == "true"
            };

            configFields.Add(fieldInfo);
        }

        return configFields;
    }

    private string GenerateConfigClass(INamedTypeSymbol classSymbol, List<ConfigFieldInfo> configFields, 
        string fileName, string dirPath, string description, string version)
    {
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var privateFieldName = ToLowerFirstChar(className);

        var properties = new StringBuilder();
        var initialization = new StringBuilder();
        var saveMethod = new StringBuilder();
        var loadMethod = new StringBuilder();

        // 生成属性
        foreach (var field in configFields)
        {
            var privateField = $"{ToLowerFirstChar(field.FieldName)}";
            var propertyName = field.Name;
            var fieldType = field.FieldType;
            
            // 生成YamlMember属性
            var yamlMemberAttributes = new List<string>();
            if (!string.IsNullOrEmpty(field.Description))
            {
            yamlMemberAttributes.Add($"Description = \"{field.Description}\"");
            }
            // 如果指定了Alias，使用Alias；否则根据ApplyNamingConventions决定
            if (!string.IsNullOrEmpty(field.Alias))
            {
                yamlMemberAttributes.Add($"Alias = \"{field.Alias}\"");
            }
            if (field.ApplyNamingConventions)
            {
                yamlMemberAttributes.Add($"ApplyNamingConventions = \"{field.ApplyNamingConventions}\"");
            }

            var yamlMemberAttribute = $"[global::YamlDotNet.Serialization.YamlMember({string.Join(", ", yamlMemberAttributes)})]";

            properties.AppendLine($$"""
                                    /// <summary>
                                    /// {{field.Description}}
                                    /// </summary>
                                    {{yamlMemberAttribute}}
                                    public {{fieldType}} {{propertyName}}
                                    {
                                        get => {{privateField}};
                                        set
                                        {
                                            if (!global::System.Collections.Generic.EqualityComparer<{{fieldType}}>.Default.Equals({{privateField}}, value))
                                            {
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                if (!_initialized) return;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                                SaveToYaml();
                                            }
                                        }
                                    }
                                """);
        }


        return $$"""
                 // Automatic Generate From ShadowPluginLoader.SourceGenerator
                 using global::System.ComponentModel;
                 using global::System.Runtime.CompilerServices;
                 using global::Newtonsoft.Json;
                 
                 namespace {{namespaceName}};

                 /// <summary>
                 /// {{description}}
                 /// Version: {{version}}
                 /// </summary>
                 public partial class {{className}} : global::ShadowPluginLoader.WinUI.Models.BaseConfig
                 { 
                     /// <summary>
                     /// The name of the configuration file
                     /// </summary>
                     [global::YamlDotNet.Serialization.YamlIgnore] 
                     protected override string FileName => "{{fileName}}";
                     
                     /// <summary>
                     /// The path of the configuration file
                     /// </summary>
                     [global::YamlDotNet.Serialization.YamlIgnore] 
                     protected override string ConfigPath { get; }

                     /// <summary>
                     /// The name of the configuration directory
                     /// </summary>
                     [global::YamlDotNet.Serialization.YamlIgnore] 
                     protected override string DirectoryName => "{{dirPath}}";

                     /// <summary>
                     /// Whether the configuration has been initialized
                     /// </summary>
                     private bool _initialized = false;
                     
                     /// <summary>
                     /// Constructor
                     /// </summary>
                     public {{className}}()
                     {
                         ConfigPath = global::System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DirectoryName, FileName);
                         var configDir = global::System.IO.Path.GetDirectoryName(ConfigPath);
                         if (!global::System.IO.Directory.Exists(configDir))
                         {
                             global::System.IO.Directory.CreateDirectory(configDir);
                         }
                         LoadFromYamlOrCreate<{{className}}>();
                         _initialized = true;
                         AfterConfigInit();
                     }
                     
                     partial void AfterConfigInit();

                 {{properties}}

                 {{saveMethod}}

                 {{loadMethod}}
                 }
                 """;
    }

    private string GetAttributeValue(AttributeData attribute, string propertyName, object defaultValue)
    {
        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == propertyName)
            {
                return namedArgument.Value.Value?.ToString() ?? defaultValue?.ToString() ?? "";
            }
        }
        return defaultValue?.ToString() ?? "";
    }
 
    private static string ToLowerFirstChar(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

    private INamedTypeSymbol? FindConfigClassInSameNamespace(INamedTypeSymbol mainPluginSymbol, 
        List<ClassDeclarationSyntax> configClasses, GeneratorExecutionContext context)
    {
        var mainPluginNamespace = mainPluginSymbol.ContainingNamespace.ToDisplayString();
        
        foreach (var configDeclaration in configClasses)
        {
            var semanticModel = context.Compilation.GetSemanticModel(configDeclaration.SyntaxTree);
            var configSymbol = semanticModel.GetDeclaredSymbol(configDeclaration) as INamedTypeSymbol;
            
            if (configSymbol == null) continue;
            
            // 检查是否在同一命名空间
            if (configSymbol.ContainingNamespace.ToDisplayString() == mainPluginNamespace)
            {
                return configSymbol;
            }
        }
        
        return null;
    }

    private string GenerateMainPluginConfigExtension(INamedTypeSymbol mainPluginSymbol, INamedTypeSymbol configSymbol)
    {
        var namespaceName = mainPluginSymbol.ContainingNamespace.ToDisplayString();
        var mainPluginClassName = mainPluginSymbol.Name;
        var configClassName = configSymbol.Name;

        return $$"""
                 // Automatic Generate From ShadowPluginLoader.SourceGenerator
                 
                 namespace {{namespaceName}};
                 
                 public partial class {{mainPluginClassName}}
                 {
                     /// <summary>
                     /// 配置实例
                     /// </summary>
                     public static {{configClassName}} Config { get; } = new {{configClassName}}();
                 }
                 """;
    }

    private class ConfigFieldInfo
    {
        public string FieldName { get; set; } = "";
        public string FieldType { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Alias { get; set; } = "";
        public bool ApplyNamingConventions { get; set; } = true;
    }
}
