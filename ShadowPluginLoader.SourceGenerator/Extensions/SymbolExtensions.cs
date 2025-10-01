using Microsoft.CodeAnalysis;

namespace ShadowPluginLoader.SourceGenerator.Extensions;

/// <summary>
/// Symbol Extensions
/// </summary>
internal static class SymbolExtensions
{
    /// <summary>
    /// Check if symbol has attribute
    /// </summary>
    /// <param name="symbol">Symbol to check</param>
    /// <param name="context">Generator context</param>
    /// <param name="attributeName">Attribute name</param>
    /// <returns>True if has attribute</returns>
    public static bool HasAttribute(this ISymbol symbol, GeneratorExecutionContext context, string attributeName)
    {
        return symbol.GetAttributes().Any(attr => 
            attr.AttributeClass?.ToDisplayString() == attributeName);
    }

    /// <summary>
    /// Check if symbol has attribute
    /// </summary>
    /// <param name="symbol">Symbol to check</param>
    /// <param name="compilation">Compilation</param>
    /// <param name="attributeName">Attribute name</param>
    /// <returns>True if has attribute</returns>
    public static bool HasAttribute(this ISymbol symbol, Compilation compilation, string attributeName)
    {
        return symbol.GetAttributes().Any(attr => 
            attr.AttributeClass?.ToDisplayString() == attributeName);
    }
}
