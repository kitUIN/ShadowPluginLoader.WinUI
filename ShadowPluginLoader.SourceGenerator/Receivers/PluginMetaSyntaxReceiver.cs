using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

public class PluginMetaSyntaxReceiver : ISyntaxReceiver
{
    public ClassDeclarationSyntax? Plugin { get; private set; }
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        CandidateClasses.Add(classDeclaration);
        // 检查类是否具有 MainPluginAttribute 特性
        foreach (var attribute in classDeclaration
                     .AttributeLists
                     .SelectMany(attributeList => attributeList.Attributes))
        {
            if (attribute.Name.ToString() == "MainPlugin")
            {
                Plugin = classDeclaration;
            }
        }
    }
}