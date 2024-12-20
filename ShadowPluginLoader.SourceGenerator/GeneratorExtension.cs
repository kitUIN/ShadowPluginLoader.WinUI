using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowPluginLoader.SourceGenerator;

internal static class GeneratorExtension
{
    /// <summary>
    /// Checks if the specified attribute is present on any class within the compilation context.
    /// </summary>
    /// <param name="context">The source generator execution context.</param>
    /// <param name="attributeName">The fully qualified metadata name of the attribute to check for.</param>
    /// <returns>True if the attribute is found on any class in the compilation; otherwise, false.</returns>
    public static bool CheckAttribute(this GeneratorExecutionContext context, string attributeName)
    {
        var compilation = context.Compilation;

        var serializableSymbol =
            compilation.GetTypeByMetadataName(attributeName);

        return (from tree in compilation.SyntaxTrees
            let model = compilation.GetSemanticModel(tree)
            let classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            where classes.Any<ClassDeclarationSyntax>(cls =>
                model.GetDeclaredSymbol(cls)!.GetAttributes().Any(a =>
                    a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default)))
            select model).Any();
    }

    public static bool HasAttribute(this ISymbol symbol, GeneratorExecutionContext context, string attributeName)
    {
        var serializableSymbol =
            context.Compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .Any(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
    }

    public static T GetAttributeConstructorArgument<T>(this ISymbol symbol,  GeneratorExecutionContext context, string attributeName, int argumentIndex)
    {
        var serializableSymbol =
            context.Compilation.GetTypeByMetadataName(attributeName);
        var attributeData = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
        if (attributeData == null || attributeData.ConstructorArguments.Length <= argumentIndex)
            return default!;
        return (T)attributeData.ConstructorArguments[argumentIndex].Value!;
    }
}