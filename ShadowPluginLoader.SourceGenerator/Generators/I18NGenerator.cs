using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowPluginLoader.SourceGenerator.Models;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace ShadowPluginLoader.SourceGenerator.Generators;

internal class GeneratorData
{
    public ImmutableArray<ClassDeclarationSyntax> ClassDeclarations { get; set; }
    public ImmutableArray<RecordDeclarationSyntax> RecordDeclarations { get; set; }
    public Compilation Compilation { get; set; } = null!;
    public ImmutableArray<AdditionalText> AdditionalTexts { get; set; }
}

/// <summary>
/// Auto Generate I18N
/// </summary>
[Generator]
internal class I18NGenerator : IIncrementalGenerator
{
    private static Dictionary<string, List<ReswValue>> GetReswData(ImmutableArray<AdditionalText> additionalTexts)
    {
        var reswData = new Dictionary<string, List<ReswValue>>();
        foreach (var file in additionalTexts)
        {
            if (!Path.GetExtension(file.Path).Equals(".resw", StringComparison.OrdinalIgnoreCase)) continue;
            var country = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(file.Path));
            var doc = XDocument.Load(file.Path);
            if (doc.Root == null) continue;
            var elements = doc.Root.Elements("data");
            foreach (var data in elements)
            {
                var name = data.Attribute("name");
                if (name == null) continue;
                var comment = data.Element("comment");
                var value = data.Element("value");
                if (!reswData.ContainsKey(name.Value))
                    reswData.Add(name.Value, new List<ReswValue>());
                reswData[name.Value].Add(new ReswValue
                {
                    Country = country ?? string.Empty,
                    Value = value?.Value ?? string.Empty,
                    Comment = comment?.Value ?? null
                });
            }
        }

        return reswData;
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 创建语法提供器来查找类声明和记录声明
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(classDecl => classDecl != null);

        var recordDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is RecordDeclarationSyntax,
                transform: (ctx, _) => (RecordDeclarationSyntax)ctx.Node)
            .Where(recordDecl => recordDecl != null);

        // 创建编译提供器
        var compilationProvider = context.CompilationProvider;
        var additionalTextsProvider = context.AdditionalTextsProvider;

        // 组合所有提供器
        var combinedProvider = classDeclarations
            .Collect()
            .Combine(recordDeclarations.Collect())
            .Combine(compilationProvider)
            .Combine(additionalTextsProvider.Collect())
            .Select((data, _) => new GeneratorData
            {
                ClassDeclarations = data.Left.Left.Left,
                RecordDeclarations = data.Left.Left.Right,
                Compilation = data.Left.Right,
                AdditionalTexts = data.Right
            });

        // 注册源生成
        context.RegisterSourceOutput(combinedProvider, Execute);
    }

    private static void Execute(SourceProductionContext context, GeneratorData data)
    {
        var logger = new Logger("I18nGenerator", context);
        var classDeclarations = data.ClassDeclarations;
        var recordDeclarations = data.RecordDeclarations;
        var compilation = data.Compilation;
        var additionalTexts = data.AdditionalTexts;

        // 获取根命名空间
        var currentNamespace = compilation.AssemblyName;
        if (currentNamespace is null) return;

        var assemblyName = compilation.AssemblyName;
        var isPlugin = false;
        var isPluginLoader = false;
        var builtIn = false;

        // 检查类声明中的 MainPlugin 特性
        foreach (var classDeclaration in classDeclarations)
        {
            var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol mainPluginSymbol)
            {
                var mainPluginAttribute = GetAttribute(mainPluginSymbol, compilation, "ShadowPluginLoader.Attributes.MainPluginAttribute");
                if (mainPluginAttribute != null)
                {
                    isPlugin = true;
                    foreach (var namedArgument in mainPluginAttribute.NamedArguments)
                    {
                        var name = namedArgument.Key;
                        if (namedArgument.Value.Value == null) continue;
                        if (name == "BuiltIn") builtIn = (bool)namedArgument.Value.Value;
                    }
                }
            }
        }

        // 检查记录声明中的 ExportMeta 特性
        foreach (var recordDeclaration in recordDeclarations)
        {
            var model = compilation.GetSemanticModel(recordDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(recordDeclaration) is INamedTypeSymbol metaSymbol &&
                GetAttribute(metaSymbol, compilation, "ShadowPluginLoader.Attributes.ExportMetaAttribute") != null)
            {
                isPluginLoader = true;
            }
        }

        var resws = GetReswData(additionalTexts);
        if (resws.Count == 0)
        {
            logger.Warning("SPLW001", "No .resw file found, skip I18N generation.");
            return;
        }

        var keyList = new List<string>();
        var i18NList = new List<string>();
        foreach (var resw in resws)
        {
            List<string> r = [];
            foreach (var x in resw.Value)
            {
                var a = $"{x.Country}:{x.Value}";
                if (x.Comment != null)
                {
                    a += $"({x.Comment})";
                }

                r.Add(a);
            }

            var enums = string.Join("\n        ///", r);
            keyList.Add($@"
        /// <summary>
        /// {enums}
        /// </summary>
        {resw.Key}");
            i18NList.Add($@"
        /// <summary>
        /// {enums}
        /// </summary>
        public static string {resw.Key} => ResourcesHelper.GetString(ResourceKey.{resw.Key});");
        }

        var keys = string.Join(",", keyList);
        var i18Ns = string.Join("", i18NList);
        var reswEnumCode = $$"""
                             // Automatic Generate From ShadowPluginLoader.SourceGenerator
                             namespace {{currentNamespace}}.I18n
                             {
                                 /// <summary>
                                 /// Resource Key
                                 /// </summary>
                                 public enum ResourceKey
                                 {
                                     {{keys}}
                                 }
                             }
                             """;
        context.AddSource($"ResourceKey.g.cs", reswEnumCode);
        string? resourcesHelperCode;
        if (isPluginLoader || builtIn)
        {
            resourcesHelperCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using Microsoft.Windows.ApplicationModel.Resources;

                                    namespace {{currentNamespace}}.I18n
                                    {
                                        /// <summary>
                                        /// Resource Helper
                                        /// </summary>
                                        public static class ResourcesHelper
                                        {
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            private static readonly ResourceManager resourceManager = new();
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(string key)
                                            {
                                                return resourceManager.MainResourceMap.GetValue("{{assemblyName}}/Resources/" + key).ValueAsString;
                                            }
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(ResourceKey key)
                                            {
                                                return GetString(key.ToString());
                                            }
                                        }
                                    }
                                    """;
        }
        else if (isPlugin)
        {
            resourcesHelperCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using CustomExtensions.WinUI;
                                    using Windows.ApplicationModel.Resources.Core;

                                    namespace {{currentNamespace}}.I18n
                                    {
                                        /// <summary>
                                        /// Resource Helper
                                        /// </summary>
                                        public static class ResourcesHelper
                                        {
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(string key) 
                                            {
                                                var resourceManager = ApplicationExtensionHost.GetResourceMapForAssembly();
                                                return resourceManager.GetValue(key).ValueAsString;
                                            }
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(ResourceKey key)
                                            {
                                                return GetString(key.ToString());
                                            }
                                        }
                                    }
                                    """;
        }
        else
        {
            resourcesHelperCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using Microsoft.Windows.ApplicationModel.Resources;

                                    namespace {{currentNamespace}}.I18n
                                    {
                                        /// <summary>
                                        /// Resources Helper
                                        /// </summary>
                                        public static class ResourcesHelper
                                        {
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            private static readonly ResourceLoader resourceLoader = new ResourceLoader();
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(string key)
                                            {
                                                return resourceLoader.GetString(key);
                                            }
                                            /// <summary>
                                            /// 
                                            /// </summary>
                                            public static string GetString(ResourceKey key)
                                            {
                                                return GetString(key.ToString());
                                            }
                                        }
                                    }
                                    """;
        }

        context.AddSource($"ResourcesHelper.g.cs", resourcesHelperCode);
        var i18NCode = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator

                         namespace {{currentNamespace}}.I18n
                         {
                             /// <summary>
                             /// I18N
                             /// </summary>
                             public static class I18N
                             {
                                 {{i18Ns}}
                             }
                         }
                         """;
        context.AddSource($"I18N.g.cs", i18NCode);

        var localeExtensionCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using Microsoft.UI.Xaml.Markup;

                                    namespace {{currentNamespace}}.I18n
                                    {
                                        /// <summary>
                                        /// I18N Xaml Extension
                                        /// </summary>
                                        [MarkupExtensionReturnType(ReturnType = typeof(string))]
                                        public sealed class LocaleExtension : MarkupExtension
                                        {
                                            
                                            /// <summary>
                                            /// Key
                                            /// </summary>
                                            public ResourceKey Key { get; set; }
                                    
                                            /// <inheritdoc/>
                                            protected override object ProvideValue()
                                            {
                                                return ResourcesHelper.GetString(Key);
                                            }
                                        }
                                    }
                                    """;
        context.AddSource($"LocaleExtension.g.cs", localeExtensionCode);
    }

    private static AttributeData? GetAttribute(ISymbol symbol, Compilation compilation, string attributeName)
    {
        var attributeSymbol = compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default));
    }
}