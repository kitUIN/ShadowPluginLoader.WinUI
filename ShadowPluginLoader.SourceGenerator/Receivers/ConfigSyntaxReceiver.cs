using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

public class ConfigSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ConfigClasses { get; } = [];
    public List<ClassDeclarationSyntax> MainPluginClasses { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        
        // 检查是否是分部类
        if (!classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))) return;
        
        // 检查是否有ConfigAttribute
        if (HasConfigAttribute(classDeclaration))
        {
            ConfigClasses.Add(classDeclaration);
        }
        
        // 检查是否有MainPluginAttribute
        if (HasMainPluginAttribute(classDeclaration))
        {
            MainPluginClasses.Add(classDeclaration);
        }
    }

    private static bool HasConfigAttribute(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString() == "Config");
    }
    
    private static bool HasMainPluginAttribute(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString() == "MainPlugin");
    }
}
