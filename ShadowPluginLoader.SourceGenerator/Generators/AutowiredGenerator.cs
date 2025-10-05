using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowPluginLoader.SourceGenerator.Extensions;
using ShadowPluginLoader.SourceGenerator.Helpers;
using ShadowPluginLoader.SourceGenerator.Models;

namespace ShadowPluginLoader.SourceGenerator.Generators;

/// <summary>
/// Autowired Generator - Incremental Source Generator
/// </summary>
[Generator]
public class AutowiredGenerator : IIncrementalGenerator
{
    private static string ToLowerFirst(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 创建语法提供器来查找类声明
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(classDecl => classDecl != null);

        // 创建编译提供器
        var compilationProvider = context.CompilationProvider;

        // 组合提供器
        var combinedProvider = classDeclarations
            .Collect()
            .Combine(compilationProvider)
            .Select((data, _) => new GeneratorData
            {
                ClassDeclarations = data.Left,
                Compilation = data.Right
            });

        // 注册源生成
        context.RegisterSourceOutput(combinedProvider, Execute);
    }

    private static void Execute(SourceProductionContext context, GeneratorData data)
    {
        var logger = new Logger("AutowiredGenerator", context);
        var classDeclarations = data.ClassDeclarations;
        var compilation = data.Compilation;

        if (classDeclarations.IsEmpty)
        {
            logger.Warning("SPLW003", "No classes found, skip Autowired generation.");
            return;
        }

        // 过滤出有 Autowired 属性或 CheckAutowired 属性的类
        var autowiredClasses = new List<ClassDeclarationSyntax>();
        foreach (var classDeclaration in classDeclarations)
        {
            var hasAutowiredProperties = classDeclaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Any(prop => prop.AttributeLists
                    .SelectMany(attrList => attrList.Attributes)
                    .Any(attr => attr.Name.ToString().Contains("Autowired")));

            var hasCheckAutowiredAttribute = classDeclaration.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString().Contains("CheckAutowired"));

            if (hasAutowiredProperties || hasCheckAutowiredAttribute)
            {
                autowiredClasses.Add(classDeclaration);
            }
        }

        if (autowiredClasses.Count == 0)
        {
            logger.Warning("SPLW003", "No Autowired Class found, skip Autowired generation.");
            return;
        }

        var sortedClasses = InheritanceSorter.SortTypesByInheritance(autowiredClasses
            .Select(classSyntax =>
                compilation
                    .GetSemanticModel(classSyntax.SyntaxTree)
                    .GetDeclaredSymbol(classSyntax))
            .OfType<INamedTypeSymbol>());
        // 用于存储基类构造函数信息的字典
        var baseConstructors = new Dictionary<string, List<BaseConstructor>>();

        foreach (var classSymbol in sortedClasses)
        {
            var needCheck =
                classSymbol.HasAttribute(compilation, "ShadowPluginLoader.Attributes.CheckAutowiredAttribute");
            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>().Where(p => p.HasAttribute(compilation,
                    "ShadowPluginLoader.Attributes.AutowiredAttribute"));
            var propertySymbols = properties as IPropertySymbol[] ?? properties.ToArray();
            if (!needCheck && !propertySymbols.Any()) continue;
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var classClassNameSafe = classSymbol.Name;
            var classClassName = classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var constructors = new List<string>();
            var assignments = new List<string>();
            var baseConstructorParams = new List<string>();
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
                var baseTypeKey = baseTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (baseConstructors.ContainsKey(baseTypeKey))
                {
                    foreach (var parameter in baseConstructors[baseTypeKey])
                    {
                        constructors.Add($"{parameter.Type} {parameter.Name}");
                        baseConstructorParams.Add($"{parameter.Name}");
                    }
                }
                else
                {
                    var baseConstructor = baseTypeSymbol.GetMembers()
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
                            baseConstructorParams.Add($"{propertySmallName}");
                        }
                    }
                }

                if (baseConstructorParams.Count > 0)
                {
                    baseConstructorString = " : base(" + string.Join(", ", baseConstructorParams) + ")";
                }
            }

            if (constructors.Count == 0) continue;
            var baseConstructorsKey = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            baseConstructors[baseConstructorsKey] = constructorRecord;
            var code = $$"""
                         // Automatic Generate From ShadowPluginLoader.SourceGenerator

                         namespace {{namespaceName}};

                         public partial class {{classClassName}}
                         {
                             /// <summary>
                             /// 
                             /// </summary>
                             public {{classClassNameSafe}}({{string.Join(", ", constructors)}}){{baseConstructorString}}
                             {
                                 {{string.Join("\n        ", assignments)}}
                                 ConstructorInit();
                             }
                             
                             /// <summary>
                             /// Constructor Init
                             /// </summary>
                             partial void ConstructorInit();
                         }
                         """;
            context.AddSource($"{classClassNameSafe}_Autowired.g.cs", code);
        }
    }
}