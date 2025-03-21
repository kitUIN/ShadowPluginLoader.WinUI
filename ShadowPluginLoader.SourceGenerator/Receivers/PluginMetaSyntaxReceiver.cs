using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

public class PluginMetaSyntaxReceiver : ISyntaxReceiver
{
    public ClassDeclarationSyntax? Plugin { get; private set; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        // 检查类是否具有 MainPluginAttribute 特性
        var hasMainPluginAttribute = classDeclaration
            .AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.ToString() == "MainPluginAttribute");
        if (hasMainPluginAttribute)
        {
            Plugin = classDeclaration;
        }
    }
}