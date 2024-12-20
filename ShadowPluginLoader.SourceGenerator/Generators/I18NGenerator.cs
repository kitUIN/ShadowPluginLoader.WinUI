﻿using Microsoft.CodeAnalysis;
using ShadowPluginLoader.SourceGenerator.Models;
using System.Xml.Linq;

namespace ShadowPluginLoader.SourceGenerator.Generators;

/// <summary>
/// Auto Generate I18N
/// </summary>
[Generator]
internal class I18NGenerator : ISourceGenerator
{
    private static Dictionary<string, List<ReswValue>> GetReswData(GeneratorExecutionContext context)
    {
        var reswData = new Dictionary<string, List<ReswValue>>();
        foreach (var file in context.AdditionalFiles)
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

    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("I18nGenerator", context);
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.RootNamespace",
            out var currentNamespace);
        if (currentNamespace is null) return;
        var isPlugin = context.CheckAttribute("ShadowPluginLoader.MetaAttributes.AutoPluginMetaAttribute");
        var isPluginLoader = context.CheckAttribute("ShadowPluginLoader.MetaAttributes.ExportMetaAttribute");

        var resws = GetReswData(context);
        if (resws.Count == 0)
        {
            logger.Warning("SPLW001", "No .resw file found, skip I18N generation.");
            return;
        }

        var keys = string.Empty;
        var i18ns = string.Empty;
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

        keys = string.Join(",", keyList);
        i18ns = string.Join("", i18NList);
        var reswEnumCode = $$"""
                             // Automatic Generate From ShadowPluginLoader.SourceGenerator
                             namespace {{currentNamespace}}.Enums
                             {
                                 internal enum ResourceKey
                                 {
                                     {{keys}}
                                 }
                             }
                             """;
        context.AddSource($"ResourceKey.g.cs", reswEnumCode);
        string? resourcesHelperCode;
        if (isPluginLoader)
        {
            resourcesHelperCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using {{currentNamespace}}.Enums;
                                    using Microsoft.Windows.ApplicationModel.Resources;

                                    namespace {{currentNamespace}}.Helpers
                                    {
                                        internal static class ResourcesHelper
                                        {
                                            private static readonly ResourceManager resourceManager = new();
                                            public static string GetString(string key)
                                            {
                                                return resourceManager.MainResourceMap.GetValue("{{currentNamespace}}/Resources/" + key).ValueAsString;
                                            }
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
                                    using {{currentNamespace}}.Enums;
                                    using Windows.ApplicationModel.Resources.Core;

                                    namespace {{currentNamespace}}.Helpers
                                    {
                                        internal static class ResourcesHelper
                                        {
                                            // private static readonly ResourceMap resourceManager = ApplicationExtensionHost.GetResourceMapForAssembly();
                                            public static string GetString(string key) 
                                            {
                                                var resourceManager = ApplicationExtensionHost.GetResourceMapForAssembly();
                                                return resourceManager.GetValue(key).ValueAsString;
                                            }
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
                                    using {{currentNamespace}}.Enums;
                                    using Microsoft.Windows.ApplicationModel.Resources;

                                    namespace {{currentNamespace}}.Helpers
                                    {
                                        internal static class ResourcesHelper
                                        {
                                            private static readonly ResourceLoader resourceLoader = new ResourceLoader();
                                            public static string GetString(string key)
                                            {
                                                return resourceLoader.GetString(key);
                                            }
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
                         using {{currentNamespace}}.Enums;
                         namespace {{currentNamespace}}.Helpers
                         {
                             /// <summary>
                             /// I18N
                             /// </summary>
                             internal static class I18N
                             {
                                 {{i18ns}}
                             }
                         }
                         """;
        context.AddSource($"I18N.g.cs", i18NCode);

        var localeExtensionCode = $$"""
                                    // Automatic Generate From ShadowPluginLoader.SourceGenerator
                                    using Microsoft.UI.Xaml.Markup;
                                    using {{currentNamespace}}.Helpers;
                                    using {{currentNamespace}}.Enums;
                                    namespace {{currentNamespace}}.Extensions
                                    {
                                        /// <summary>
                                        /// I18N Xaml Extension
                                        /// </summary>
                                        [MarkupExtensionReturnType(ReturnType = typeof(string))]
                                        internal sealed class LocaleExtension : MarkupExtension
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

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}