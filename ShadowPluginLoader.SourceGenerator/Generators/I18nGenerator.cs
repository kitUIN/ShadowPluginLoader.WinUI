using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowPluginLoader.SourceGenerator.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace ShadowPluginLoader.SourceGenerator.Generators;

/// <summary>
/// Auto Generate I18N
/// </summary>
[Generator]
internal class I18nGenerator : ISourceGenerator
{
    static Dictionary<string, List<ReswValue>> GetReswDatas(GeneratorExecutionContext context)
    {
        var reswDatas = new Dictionary<string, List<ReswValue>>();
        foreach (AdditionalText file in context.AdditionalFiles)
        {
            if (Path.GetExtension(file.Path).Equals(".resw", StringComparison.OrdinalIgnoreCase))
            {
                var country = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(file.Path));
                var doc = XDocument.Load(file.Path);
                var datas = doc.Root.Elements("data");
                foreach (var data in datas)
                {
                    var name = data.Attribute("name");
                    if (name == null) continue;
                    var comment = data.Element("comment");
                    var value = data.Element("value");
                    if (!reswDatas.ContainsKey(name.Value))
                        reswDatas.Add(name.Value, new List<ReswValue>());
                    reswDatas[name.Value].Add(new ReswValue
                    {
                        Country = country ?? string.Empty,
                        Value = value?.Value ?? string.Empty,
                        Comment = comment?.Value ?? null
                    });
                }
            }
        }
        return reswDatas;
    }
    private bool ContainsMetadataName(GeneratorExecutionContext context,string name)
    {
        var compilation = context.Compilation;

        var serializableSymbol =
            compilation.GetTypeByMetadataName(name);

        foreach (var tree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var cls in classes)
            {
                if (model.GetDeclaredSymbol(cls)!.GetAttributes().Any(a =>
                        a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CheckIsPluginLoader(GeneratorExecutionContext context)
    {
        return ContainsMetadataName(context, "ShadowPluginLoader.MetaAttributes.ExportMetaAttribute");
    }
    private bool CheckIsPlugin(GeneratorExecutionContext context)
    {
        return ContainsMetadataName(context, "ShadowPluginLoader.MetaAttributes.AutoPluginMetaAttribute");
    }
    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("I18nGenerator", context);
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.RootNamespace", out var currentNamespace);
        if (currentNamespace is null) return;
        var isPlugin = CheckIsPlugin(context);
        var isPluginLoader = CheckIsPluginLoader(context);
        
        var resws = GetReswDatas(context);
        if (resws.Count == 0) {
            logger.Warning("SPLW001", "No .resw file found, skip I18N generation.");
            return; 
        }
        string keys = string.Empty;
        string i18ns = string.Empty;
        var keyList = new List<string>();
        var i18nList = new List<string>();
        foreach (var resw in resws)
        {
            List<string> r = new();
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
            i18nList.Add($@"
        /// <summary>
        /// {enums}
        /// </summary>
        public static string {resw.Key} => ResourcesHelper.GetString(ResourceKey.{resw.Key});");
        }
        keys = string.Join(",", keyList);
        i18ns = string.Join("", i18nList);
        var reswEnumCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
namespace {currentNamespace}.Enums
{{
    internal enum ResourceKey
    {{
        {keys}
    }}
}}";
        context.AddSource($"ResourceKey.g.cs", reswEnumCode);
        string? resourcesHelperCode;
        if (isPluginLoader)
        {
            resourcesHelperCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using {currentNamespace}.Enums;
using Microsoft.Windows.ApplicationModel.Resources;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        private static readonly ResourceManager resourceManager = new();
        public static string GetString(string key)
        {{
            return resourceManager.MainResourceMap.GetValue(""{currentNamespace}/Resources/"" + key).ValueAsString;
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";
        }
        else if (isPlugin)
        {
            resourcesHelperCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using CustomExtensions.WinUI;
using {currentNamespace}.Enums;
using Windows.ApplicationModel.Resources.Core;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        // private static readonly ResourceMap resourceManager = ApplicationExtensionHost.GetResourceMapForAssembly();
        public static string GetString(string key) 
        {{
            var resourceManager = ApplicationExtensionHost.GetResourceMapForAssembly();
            return resourceManager.GetValue(key).ValueAsString;
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";
        }
        else
        {
            resourcesHelperCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using {currentNamespace}.Enums;
using Microsoft.Windows.ApplicationModel.Resources;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        private static readonly ResourceLoader resourceLoader = new ResourceLoader();
        public static string GetString(string key)
        {{
            return resourceLoader.GetString(key);
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";

        }
        context.AddSource($"ResourcesHelper.g.cs", resourcesHelperCode);
        var i18nCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using {currentNamespace}.Enums;
namespace {currentNamespace}.Helpers
{{
    /// <summary>
    /// I18N
    /// </summary>
    internal static class I18N
    {{
        {i18ns}
    }}
}}";
        context.AddSource($"I18N.g.cs", i18nCode);

        var localeExtensionCode = $@"// Automatic Generate From ShadowPluginLoader.SourceGenerator
using Microsoft.UI.Xaml.Markup;
using {currentNamespace}.Helpers;
using {currentNamespace}.Enums;
namespace {currentNamespace}.Extensions
{{
    /// <summary>
    /// I18N Xaml Extension
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    internal sealed class LocaleExtension : MarkupExtension
    {{
        
        /// <summary>
        /// Key
        /// </summary>
        public ResourceKey Key {{ get; set; }}

        /// <inheritdoc/>
        protected override object ProvideValue()
        {{
            return ResourcesHelper.GetString(Key);
        }}
    }}
}}";
        context.AddSource($"LocaleExtension.g.cs", localeExtensionCode);

    }
    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
