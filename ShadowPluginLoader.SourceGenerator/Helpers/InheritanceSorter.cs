using Microsoft.CodeAnalysis;

namespace ShadowPluginLoader.SourceGenerator.Helpers;

/// <summary>
/// Inheritance Sorter Helper
/// </summary>
internal static class InheritanceSorter
{
    /// <summary>
    /// Sort Types By Inheritance
    /// </summary>
    /// <param name="types">Types to sort</param>
    /// <returns>Sorted types by inheritance order</returns>
    public static IEnumerable<INamedTypeSymbol> SortTypesByInheritance(IEnumerable<INamedTypeSymbol> types)
    {
        var typeList = types.ToList();
        var sorted = new List<INamedTypeSymbol>();
        var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var type in typeList)
        {
            SortTypeRecursive(type, typeList, sorted, visited);
        }

        return sorted;
    }

    private static void SortTypeRecursive(INamedTypeSymbol type, List<INamedTypeSymbol> allTypes, 
        List<INamedTypeSymbol> sorted, HashSet<INamedTypeSymbol> visited)
    {
        if (visited.Contains(type) || sorted.Any(t => SymbolEqualityComparer.Default.Equals(t, type)))
            return;

        visited.Add(type);

        // 先处理基类
        if (type.BaseType != null && type.BaseType.SpecialType != SpecialType.System_Object)
        {
            var baseType = allTypes.FirstOrDefault(t => 
                SymbolEqualityComparer.Default.Equals(t, type.BaseType));
            if (baseType != null)
            {
                SortTypeRecursive(baseType, allTypes, sorted, visited);
            }
        }

        // 再添加当前类型
        if (!sorted.Contains(type))
        {
            sorted.Add(type);
        }
    }
}
