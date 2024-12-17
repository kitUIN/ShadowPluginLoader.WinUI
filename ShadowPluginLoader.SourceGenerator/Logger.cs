using Microsoft.CodeAnalysis;

namespace ShadowPluginLoader.SourceGenerator;

internal class Logger(string Category, GeneratorExecutionContext Context)
{

    public void Log(string id, string title, string message, DiagnosticSeverity severity)
    {
        var invalidXmlWarning = new DiagnosticDescriptor(id: id,
            title: title,
            messageFormat: "[{0}] {1}",
            category: Category,
            severity,
            isEnabledByDefault: true);
        Context.ReportDiagnostic(Diagnostic.Create(invalidXmlWarning, Location.None, Category, message));
    }
    public void Info(string id, string message)
    {
        Log(id, $"{Category} Info", message, DiagnosticSeverity.Info);
    }
    public void Warning(string id, string message)
    {
        Log(id, $"{Category} Warning", message, DiagnosticSeverity.Warning);
    }
    public void Error(string id, string message)
    {
        Log(id, $"{Category} Error", message, DiagnosticSeverity.Error);
    }
}