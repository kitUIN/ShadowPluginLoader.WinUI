using Microsoft.CodeAnalysis;

namespace ShadowPluginLoader.SourceGenerator;

internal class Logger
{
    private readonly string _category;
    private readonly SourceProductionContext? _incrementalContext;

    public Logger(string category, SourceProductionContext context)
    {
        _category = category;
        _incrementalContext = context;
    }

    public void Log(string id, string title, string message, DiagnosticSeverity severity)
    {
        var diagnosticDescriptor = new DiagnosticDescriptor(
            id: id,
            title: title,
            messageFormat: "[{0}] {1}",
            category: _category,
            severity,
            isEnabledByDefault: true);
        
        var diagnostic = Diagnostic.Create(diagnosticDescriptor, Location.None, _category, message);
        
        if (_incrementalContext != null)
        {
            _incrementalContext?.ReportDiagnostic(diagnostic);
        }
    }

    public void Info(string id, string message)
    {
        Log(id, $"{_category} Info", message, DiagnosticSeverity.Info);
    }

    public void Warning(string id, string message)
    {
        Log(id, $"{_category} Warning", message, DiagnosticSeverity.Warning);
    }

    public void Error(string id, string message)
    {
        Log(id, $"{_category} Error", message, DiagnosticSeverity.Error);
    }
}