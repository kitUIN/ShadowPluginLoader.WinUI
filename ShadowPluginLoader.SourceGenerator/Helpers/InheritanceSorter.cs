namespace ShadowPluginLoader.SourceGenerator.Helpers;

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
public class CustomSymbolComparer : IEqualityComparer<INamedTypeSymbol>
{
    public bool Equals(INamedTypeSymbol x, INamedTypeSymbol y)
    {
        return x.Name == y.Name && x.Kind == y.Kind;
    }

    public int GetHashCode(INamedTypeSymbol obj)
    {
        return obj.Name.GetHashCode() ^ obj.Kind.GetHashCode();
    }
}
public static class InheritanceSorter
{
    public static List<INamedTypeSymbol> SortTypesByInheritance(IEnumerable<INamedTypeSymbol> types)
    {
        // 创建一个字典存储每个类型的继承链
        var inheritanceChains = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>(new CustomSymbolComparer());

        var namedTypeSymbols = types as INamedTypeSymbol[] ?? types.ToArray();
        foreach (var type in namedTypeSymbols)
        {
            var chain = new List<INamedTypeSymbol>();
            var currentType = type;

            while (currentType != null)
            {
                chain.Add(currentType);
                currentType = currentType.BaseType;
            }
            chain.Reverse();
            inheritanceChains[type] = chain;
        }
        var sortedTypes = namedTypeSymbols.OrderBy(t => string.Join(",", inheritanceChains[t])).ToList();
        return sortedTypes;
    }
}
