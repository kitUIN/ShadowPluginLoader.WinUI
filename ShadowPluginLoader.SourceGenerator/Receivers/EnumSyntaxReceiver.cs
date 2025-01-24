using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

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