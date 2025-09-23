using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

public class ConfigSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ConfigClasses { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        
        // 检查是否是分部类
        if (!classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))) return;
        
        // 检查是否有ConfigAttribute
        if (!HasConfigAttribute(classDeclaration)) return;
        
        ConfigClasses.Add(classDeclaration);
    }

    private static bool HasConfigAttribute(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString() == "Config");
    }
}
