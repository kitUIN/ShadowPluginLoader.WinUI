using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

/// <summary>
/// 
/// </summary>
public class SettingsSyntaxReceiver : ISyntaxReceiver
{
    public List<EnumDeclarationSyntax> Enums { get; } = [];
    public ClassDeclarationSyntax Plugin { get; private set; } = null!;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is EnumDeclarationSyntax enumDeclaration)
        {
            // 收集枚举声明
            Enums.Add(enumDeclaration);
        }
        else if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        {
            // 检查类是否具有 MainPluginAttributes 特性
            var hasMainPluginAttribute = classDeclaration
                .AttributeLists
                .SelectMany(attributeList => attributeList.Attributes)
                .Any(attribute => attribute.Name.ToString() == "MainPlugin");
            if (hasMainPluginAttribute)
            {
                Plugin = classDeclaration;
            }
        }
    }
}