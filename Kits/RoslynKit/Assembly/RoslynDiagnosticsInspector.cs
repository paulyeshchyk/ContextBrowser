using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

public class RoslynDiagnosticsInspector : ICompilationDiagnosticsInspector<CSharpCompilation>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynDiagnosticsInspector(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public void LogAndFilterDiagnostics(CSharpCompilation compilation, CancellationToken cancellationToken)
    {
        var diagnostics = compilation.GetDiagnostics(cancellationToken);
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                _logger.WriteLog(AppLevel.R_Dll, LogLevel.Err, $"Diagnostics Error: {diagnostic}");
            }
            else if (diagnostic.Severity == DiagnosticSeverity.Warning)
            {
                _logger.WriteLog(AppLevel.R_Dll, LogLevel.Trace, $"Diagnostics Warning: {diagnostic}");
            }
        }
    }
}
