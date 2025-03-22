using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

/// <summary>
/// 
/// </summary>
public class I18nSyntaxReceiver : ISyntaxReceiver
{
    public ClassDeclarationSyntax? Plugin { get; private set; }
    public ClassDeclarationSyntax? ExportMeta { get; private set; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        // 检查类是否具有 MainPluginAttribute 特性
        foreach (var attribute in classDeclaration
                     .AttributeLists
                     .SelectMany(attributeList => attributeList.Attributes))
        {
            if (attribute.Name.ToString() == "MainPlugin")
            {
                Plugin = classDeclaration;
            }
            else if (attribute.Name.ToString() == "ExportMeta")
            {
                ExportMeta = classDeclaration;
            }
        }
    }
}