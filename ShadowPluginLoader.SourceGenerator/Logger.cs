using Microsoft.CodeAnalysis;

namespace ShadowPluginLoader.SourceGenerator;

internal static class Logger
{
    private static string LoggerPrefix => "[SourceGenerator]";
    
    public static void Error(GeneratorExecutionContext context, string message)
    {
        var invalidXmlWarning = new DiagnosticDescriptor(id: "Error",
            title: "Code Generator Error",
            messageFormat: "{0}",
            category: "CodeGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(invalidXmlWarning, Location.None, LoggerPrefix + message));
    }
    public static void Info(GeneratorExecutionContext context, string message)
    {
        var invalidXmlWarning = new DiagnosticDescriptor(id: "Info",
            title: "Code Generator Info",
            messageFormat: "{0}",
            category: "CodeGenerator",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(invalidXmlWarning, Location.None, LoggerPrefix + message));
    }
}