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
                var fileName = GetAttributeValue(configAttribute, "FileName", "");
                var dirPath = GetAttributeValue(configAttribute, "DirPath", "config");
                var description = GetAttributeValue(configAttribute, "Description", "");
                var version = GetAttributeValue(configAttribute, "Version", "1.0.0");
                
                // 如果FileName为空且是内部类，使用外部类的文件名
                if (string.IsNullOrEmpty(fileName) && classSymbol.ContainingType != null)
                {
                    // 内部类不生成单独文件，跳过
                    continue;
                }
                else if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "config.json";
                }

                // 查找带有ConfigFieldAttribute的字段（包括内部类）
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

        // 获取当前类的配置字段
        foreach (var member in classSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            if (!member.HasAttribute(context, configFieldAttributeName)) continue;

            var fieldAttribute = member.GetAttribute(context, configFieldAttributeName);
            if (fieldAttribute == null) continue;

            var fieldType = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var isEntityClass = IsEntityClass(member.Type, context);
            var isCollectionOfEntities = IsCollectionOfEntities(member.Type, context);

            var fieldInfo = new ConfigFieldInfo
            {
                FieldName = member.Name,
                FieldType = fieldType,
                Name = GetAttributeValue(fieldAttribute, "Name", member.Name),
                Description = GetAttributeValue(fieldAttribute, "Description", ""),
                Alias = GetAttributeValue(fieldAttribute, "Alias", ""),
                ApplyNamingConventions = GetAttributeValue(fieldAttribute, "ApplyNamingConventions", true) == "True" || GetAttributeValue(fieldAttribute, "ApplyNamingConventions", true) == "true",
                IsEntityClass = isEntityClass,
                IsCollectionOfEntities = isCollectionOfEntities
            };

            configFields.Add(fieldInfo);
        }

        // 递归查找内部类中的配置字段
        foreach (var innerType in classSymbol.GetTypeMembers().OfType<INamedTypeSymbol>())
        {
            var innerConfigFields = GetConfigFields(innerType, context);
            configFields.AddRange(innerConfigFields);
        }

        return configFields;
    }

    private string GenerateConfigClass(INamedTypeSymbol classSymbol, List<ConfigFieldInfo> configFields, 
        string fileName, string dirPath, string description, string version)
    {
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var privateFieldName = ToLowerFirstChar(className);
        
        // 如果是内部类，需要包含外部类名
        var fullClassName = className;
        if (classSymbol.ContainingType != null)
        {
            fullClassName = $"{classSymbol.ContainingType.Name}.{className}";
        }
        
        // 检查是否为内部类（FileName为空）
        var isInnerClass = classSymbol.ContainingType != null;

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

            // 根据字段类型生成不同的属性实现
            if (isInnerClass)
            {
                // 内部类生成简单的属性
                properties.AppendLine(GenerateInnerClassProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
            }
            else if (field.IsEntityClass)
            {
                properties.AppendLine(GenerateEntityProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
            }
            else if (field.IsCollectionOfEntities)
            {
                properties.AppendLine(GenerateEntityCollectionProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
            }
            else
            {
                properties.AppendLine(GenerateSimpleProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
            }
        }


        if (isInnerClass)
        {
            // 内部类也继承BaseConfig，但不生成单独文件
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
                     public partial class {{fullClassName}} : global::ShadowPluginLoader.WinUI.Models.BaseConfig
                     {
                         /// <summary>
                         /// 内部类不保存单独文件，使用外部类的文件
                         /// </summary>
                         [global::YamlDotNet.Serialization.YamlIgnore] 
                         protected override string FileName => "";

                         /// <summary>
                         /// 内部类不保存单独文件，使用外部类的文件
                         /// </summary>
                         [global::YamlDotNet.Serialization.YamlIgnore] 
                         protected override string ConfigPath => "";

                         /// <summary>
                         /// 内部类不保存单独文件，使用外部类的文件
                         /// </summary>
                         [global::YamlDotNet.Serialization.YamlIgnore] 
                         protected override string DirectoryName => "";

                         /// <summary>
                         /// 内部类不自动初始化，由外部类管理
                         /// </summary>
                         private bool _initialized = false;

                         /// <summary>
                         /// 构造函数
                         /// </summary>
                         public {{fullClassName}}()
                         {
                             // 内部类不自动加载配置，由外部类管理
                             _initialized = true;
                             InitializeEntityChangeListeners();
                             AfterConfigInit();
                         }

                         /// <summary>
                         /// 初始化实体类变更监听器
                         /// </summary>
                         private void InitializeEntityChangeListeners()
                         {
                             {{GenerateEntityListenerInitialization(configFields)}}
                         }

                     {{properties}}

                     partial void AfterConfigInit();
                     }
                     """;
        }
        else
        {
            // 外部类生成完整的配置类
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
                     public partial class {{fullClassName}} : global::ShadowPluginLoader.WinUI.Models.BaseConfig
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
                         public {{fullClassName}}()
                         {
                             ConfigPath = global::System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DirectoryName, FileName);
                             var configDir = global::System.IO.Path.GetDirectoryName(ConfigPath);
                             if (!global::System.IO.Directory.Exists(configDir))
                             {
                                 global::System.IO.Directory.CreateDirectory(configDir);
                             }
                             LoadFromYamlOrCreate<{{fullClassName}}>();
                             _initialized = true;
                             InitializeEntityChangeListeners();
                             AfterConfigInit();
                         }
                         
                         /// <summary>
                         /// 初始化实体类变更监听器
                         /// </summary>
                         private void InitializeEntityChangeListeners()
                         {
                             {{GenerateEntityListenerInitialization(configFields)}}
                         }
                         
                         partial void AfterConfigInit();

                     {{properties}}

                     {{saveMethod}}

                     {{loadMethod}}
                     }
                     """;
        }
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

    /// <summary>
    /// 检测类型是否为实体类（继承了BaseConfig）
    /// </summary>
    private bool IsEntityClass(ITypeSymbol typeSymbol, GeneratorExecutionContext context)
    {
        if (typeSymbol == null) return false;
        
        // 检查是否继承了BaseConfig
        var baseConfigType = context.Compilation.GetTypeByMetadataName("ShadowPluginLoader.WinUI.Models.BaseConfig");
        if (baseConfigType == null) return false;
        
        return typeSymbol.InheritsFrom(baseConfigType);
    }

    /// <summary>
    /// 检测类型是否为实体类集合
    /// </summary>
    private bool IsCollectionOfEntities(ITypeSymbol typeSymbol, GeneratorExecutionContext context)
    {
        if (typeSymbol == null) return false;
        
        // 检查是否为集合类型
        if (!IsCollectionType(typeSymbol)) return false;
        
        // 获取集合元素的类型
        var elementType = GetCollectionElementType(typeSymbol);
        if (elementType == null) return false;
        
        // 检查元素类型是否为实体类
        return IsEntityClass(elementType, context);
    }

    /// <summary>
    /// 检测类型是否为集合类型
    /// </summary>
    private bool IsCollectionType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null) return false;
        
        // 检查是否实现了IEnumerable接口
        var enumerableInterface = typeSymbol.AllInterfaces.FirstOrDefault(i => 
            i.Name == "IEnumerable" && i.TypeArguments.Length == 1);
        
        return enumerableInterface != null;
    }

    /// <summary>
    /// 获取集合的元素类型
    /// </summary>
    private ITypeSymbol? GetCollectionElementType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null) return null;
        
        // 查找IEnumerable<T>接口
        var enumerableInterface = typeSymbol.AllInterfaces.FirstOrDefault(i => 
            i.Name == "IEnumerable" && i.TypeArguments.Length == 1);
        
        return enumerableInterface?.TypeArguments[0];
    }

    /// <summary>
    /// 生成内部类属性（继承BaseConfig的属性）
    /// </summary>
    private string GenerateInnerClassProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
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
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                if (!_initialized) return;
                                                OnConfigChanged(nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                                // 内部类不直接保存，由外部类管理
                                            }
                                        }
                                    }
                                """;
    }

    /// <summary>
    /// 生成简单属性（非实体类）
    /// </summary>
    private string GenerateSimpleProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
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
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                if (!_initialized) return;
                                                SaveToYaml();
                                                OnConfigChanged(nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                            }
                                        }
                                    }
                                """;
    }

    /// <summary>
    /// 生成实体类属性（支持递归变更通知）
    /// </summary>
    private string GenerateEntityProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
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
                                                // 取消旧实体的变更监听
                                                if ({{privateField}} is global::ShadowPluginLoader.WinUI.Models.BaseConfig oldEntity)
                                                {
                                                    oldEntity.EntityChanged -= On{{propertyName}}EntityChanged;
                                                }
                                                
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                
                                                // 为新实体添加变更监听
                                                if ({{privateField}} is global::ShadowPluginLoader.WinUI.Models.BaseConfig newEntity)
                                                {
                                                    newEntity.EntityChanged += On{{propertyName}}EntityChanged;
                                                }
                                                
                                                if (!_initialized) return;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                                SaveToYaml();
                                            }
                                        }
                                    }
                                    
                                    /// <summary>
                                    /// {{propertyName}}实体变更事件处理
                                    /// </summary>
                                    private void On{{propertyName}}EntityChanged(object sender, global::ShadowPluginLoader.WinUI.Models.EntityChangedEventArgs e)
                                    {
                                        if (!_initialized) return;
                                        // 使用实体传递的完整路径，如果没有则构建：外部属性名.内部属性名
                                        var fullPropertyPath = string.IsNullOrEmpty(e.FullPropertyPath) 
                                            ? $"{{propertyName}}.{e.PropertyName}" 
                                            : $"{{propertyName}}.{e.FullPropertyPath}";
                                        OnConfigChanged(fullPropertyPath, e.OldValue, e.NewValue, e.EntityType);
                                        SaveToYaml();
                                    }
                                """;
    }

    /// <summary>
    /// 生成实体类集合属性（支持集合中实体的递归变更通知）
    /// </summary>
    private string GenerateEntityCollectionProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
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
                                                // 取消旧集合中所有实体的变更监听
                                                if ({{privateField}} != null)
                                                {
                                                    foreach (var item in {{privateField}})
                                                    {
                                                        if (item is global::ShadowPluginLoader.WinUI.Models.BaseConfig entity)
                                                        {
                                                            entity.EntityChanged -= On{{propertyName}}ItemEntityChanged;
                                                        }
                                                    }
                                                }
                                                
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                
                                                // 为新集合中的所有实体添加变更监听
                                                if ({{privateField}} != null)
                                                {
                                                    foreach (var item in {{privateField}})
                                                    {
                                                        if (item is global::ShadowPluginLoader.WinUI.Models.BaseConfig entity)
                                                        {
                                                            entity.EntityChanged += On{{propertyName}}ItemEntityChanged;
                                                        }
                                                    }
                                                }
                                                
                                                if (!_initialized) return;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                                SaveToYaml();
                                            }
                                        }
                                    }
                                    
                                    /// <summary>
                                    /// {{propertyName}}集合中实体变更事件处理
                                    /// </summary>
                                    private void On{{propertyName}}ItemEntityChanged(object sender, global::ShadowPluginLoader.WinUI.Models.EntityChangedEventArgs e)
                                    {
                                        if (!_initialized) return;
                                        // 使用实体传递的完整路径，如果没有则构建：集合属性名[Item].内部属性名
                                        var fullPropertyPath = string.IsNullOrEmpty(e.FullPropertyPath) 
                                            ? $"{{propertyName}}[Item].{e.PropertyName}" 
                                            : $"{{propertyName}}[Item].{e.FullPropertyPath}";
                                        OnConfigChanged(fullPropertyPath, e.OldValue, e.NewValue, e.EntityType);
                                        SaveToYaml();
                                    }
                                """;
    }

    /// <summary>
    /// 生成实体监听器初始化代码
    /// </summary>
    private string GenerateEntityListenerInitialization(List<ConfigFieldInfo> configFields)
    {
        var initialization = new StringBuilder();
        
        foreach (var field in configFields)
        {
            var privateField = ToLowerFirstChar(field.FieldName);
            var propertyName = field.Name;
            
            if (field.IsEntityClass)
            {
                initialization.AppendLine($$"""
                                            // 初始化{{propertyName}}实体变更监听
                                            if ({{privateField}} is global::ShadowPluginLoader.WinUI.Models.BaseConfig {{privateField}}Entity)
                                            {
                                                {{privateField}}Entity.EntityChanged += On{{propertyName}}EntityChanged;
                                            }
                                        """);
            }
            else if (field.IsCollectionOfEntities)
            {
                initialization.AppendLine($$"""
                                            // 初始化{{propertyName}}集合中实体变更监听
                                            if ({{privateField}} != null)
                                            {
                                                foreach (var item in {{privateField}})
                                                {
                                                    if (item is global::ShadowPluginLoader.WinUI.Models.BaseConfig entity)
                                                    {
                                                        entity.EntityChanged += On{{propertyName}}ItemEntityChanged;
                                                    }
                                                }
                                            }
                                        """);
            }
        }
        
        return initialization.ToString();
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
        public bool IsEntityClass { get; set; } = false;
        public bool IsCollectionOfEntities { get; set; } = false;
    }
}
