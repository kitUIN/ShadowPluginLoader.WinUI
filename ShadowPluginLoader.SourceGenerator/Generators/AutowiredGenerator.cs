using Microsoft.CodeAnalysis;
using ShadowPluginLoader.SourceGenerator.Helpers;
using ShadowPluginLoader.SourceGenerator.Models;
using ShadowPluginLoader.SourceGenerator.Receivers;

namespace ShadowPluginLoader.SourceGenerator.Generators;

/// <summary>
/// 
/// </summary>
[Generator]
public class AutowiredGenerator : ISourceGenerator
{
    private Dictionary<string, List<BaseConstructor>> BaseConstructors { get; } = new();

    private static string ToLowerFirst(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AutowiredClassSyntaxReceiver());
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        var logger = new Logger("AutowiredGenerator", context);
        if (context.SyntaxReceiver is not AutowiredClassSyntaxReceiver receiver)
        {
            logger.Warning("SPLW003", "No Autowired Class found, skip Autowired generation.");
            return;
        }

        if (receiver.Classes.Count == 0)
        {
            logger.Warning("SPLW003", "No Autowired Class found, skip Autowired generation.");
            return;
        }

        var sortedClasses = InheritanceSorter.SortTypesByInheritance(receiver.Classes
            .Select(classSyntax =>
                context.Compilation
                    .GetSemanticModel(classSyntax.SyntaxTree)
                    .GetDeclaredSymbol(classSyntax))
            .OfType<INamedTypeSymbol>());
        foreach (var classSymbol in sortedClasses)
        {
            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>().Where(p => p.HasAttribute(context,
                    "ShadowPluginLoader.MetaAttributes.AutowiredAttribute"));
            var propertySymbols = properties as IPropertySymbol[] ?? properties.ToArray();
            if (!propertySymbols.Any()) continue;
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var classClassName = classSymbol.Name;

            var constructors = new List<string>();
            var assignments = new List<string>();
            var baseConstructors = new List<string>();
            var constructorRecord = new List<BaseConstructor>();
            var baseConstructorString = string.Empty;

            foreach (var property in propertySymbols)
            {
                var propertyName = property.Name;
                var propertySmallName = ToLowerFirst(propertyName);
                var propertyType = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                constructors.Add($"{propertyType} {propertySmallName}");
                assignments.Add($"{propertyName} = {propertySmallName};");
                constructorRecord.Add(new BaseConstructor(propertyType, propertySmallName));
            }

            var baseTypeSymbol = classSymbol.BaseType;
            if (baseTypeSymbol != null)
            {
                if (BaseConstructors.ContainsKey(
                        baseTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                {
                    foreach (var parameter in BaseConstructors[
                                 baseTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)])
                    {
                        constructors.Add($"{parameter.Type} {parameter.Name}");
                        baseConstructors.Add($"{parameter.Name}");
                    }
                }
                else
                {
                    var baseConstructor = baseTypeSymbol?.GetMembers()
                        .OfType<IMethodSymbol>()
                        .Where(m => m.MethodKind == MethodKind.Constructor)
                        .OrderByDescending(m => m.Parameters.Length)
                        .FirstOrDefault();
                    if (baseConstructor != null && baseConstructor.Name != "object.Object()")
                    {
                        foreach (var parameter in baseConstructor.Parameters)
                        {
                            var propertyType = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var propertySmallName = ToLowerFirst(parameter.Name);
                            constructors.Add($"{propertyType} {propertySmallName}");
                            baseConstructors.Add($"{propertySmallName}");
                        }
                    }
                }

                if (baseConstructors.Count > 0)
                {
                    baseConstructorString = " : base(" + string.Join(", ", baseConstructors) + ")";
                }
            }


            if (constructors.Count == 0 || assignments.Count == 0) continue;
            var baseConstructorsKey = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            BaseConstructors[baseConstructorsKey] = constructorRecord;
            var code = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator

                         namespace {{namespaceName}};

                         public partial class {{classClassName}}
                         {
                             /// <summary>
                             /// 
                             /// </summary>
                             public {{classClassName}}({{string.Join(", ", constructors)}}){{baseConstructorString}}
                             {
                                {{string.Join("\n", assignments)}}
                             }
                         }
                         """;
            context.AddSource($"{classClassName}_Autowired.g.cs", code);
        }
    }
}