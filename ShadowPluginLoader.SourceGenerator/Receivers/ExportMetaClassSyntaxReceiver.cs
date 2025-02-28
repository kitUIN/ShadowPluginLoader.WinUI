using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator.Receivers;

public class ExportMetaClassSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> Classes { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        var hasExportMetaAttribute = classDeclaration.AttributeLists
            .SelectMany(attrList => attrList.Attributes)
            .Any(attribute => attribute.Name.ToString() == "ExportMeta");
        if (hasExportMetaAttribute)
        {
            Classes.Add(classDeclaration);
        }
    }
}