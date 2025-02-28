using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowPluginLoader.SourceGenerator.Receivers;
using ShadowPluginLoader.SourceGenerator.Models;
using Newtonsoft.Json;
using System;
using ShadowPluginLoader.SourceGenerator.Helpers;

namespace ShadowPluginLoader.SourceGenerator.Generators;

[Generator]
public class ExportMetaGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ExportMetaClassSyntaxReceiver());
    }


    private Meta GetMetaFromProperty(IPropertySymbol property)
    {
        var meta = new Meta() { PropertyGroupName = property.Name ,Type= property.Type.ToString() };
        var attributes = property.GetAttributes()
            .Where(attributeData => attributeData?.AttributeClass?.Name == "MetaAttribute");
        foreach (var attributeData in attributes)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case nameof(meta.Required):
                        meta.Required = (bool)(namedArg.Value.Value ?? true);
                        break;
                    case nameof(meta.Exclude):
                        meta.Exclude = (bool)(namedArg.Value.Value ?? false);
                        break;
                    case nameof(meta.PropertyGroupName):
                        meta.PropertyGroupName = (string)(namedArg.Value.Value ?? property.Name);
                        break;
                    case nameof(meta.Regex):
                        meta.Regex = (string?)namedArg.Value.Value;
                        break;
                }
            }
        }

        if (property.Type.ToString().EndsWith("?"))
        {
            meta.Nullable = true;
        }

        var defaultValue = GetDefaultValue(property);
        if (defaultValue != null) meta.DefaultValue = defaultValue;
        return meta;
    }

    private string? GetDefaultValue(IPropertySymbol property)
    {
        var declaringSyntaxRefs = property.DeclaringSyntaxReferences;
        foreach (var syntaxNode in declaringSyntaxRefs.Select(syntaxRef => syntaxRef.GetSyntax()))
        {
            if (syntaxNode is not PropertyDeclarationSyntax propertyDeclaration) continue;
            var initializer = propertyDeclaration.Initializer;
            if (initializer == null) continue;
            return initializer.Value.ToString();
        }

        return null;
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger(nameof(ExportMetaGenerator), context);
        if (context.SyntaxReceiver is not ExportMetaClassSyntaxReceiver receiver)
        {
            return;
        }

        if (receiver.Classes.Count > 1)
        {
            logger.Error("PL0021", "More than 1 ExportMeta Class");
            return;
        }

        if (receiver.Classes.Count == 0) return;
        var classDeclaration = receiver.Classes[0];
        var rootNamespace = context.Compilation.Assembly.Name;
        var baseType = context.Compilation
            .GetSemanticModel(classDeclaration.SyntaxTree)
            .GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
        var pluginDefine = new PluginD()
        {
            Namespace = rootNamespace,
            Type = baseType!.Name,
        };
        while (baseType != null)
        {
            var baseProperties = baseType.GetMembers().OfType<IPropertySymbol>();
            foreach (var property in baseProperties)
            {
                var meta = GetMetaFromProperty(property);
                if (meta.Exclude) continue;
                if(meta.Required) pluginDefine.Required.Add(property.Name);
                pluginDefine.Properties[property.Name] = meta;
            }
            baseType = baseType.BaseType;
            if (baseType?.Name == "Attribute") break;
        }
        var json = JsonConvert.SerializeObject(pluginDefine, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        });  
        // logger.Info("PL0101", "plugin.d.json" + json);
        context.AddSource("plugin.d.json",json);
        
    }
}