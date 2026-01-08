using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

// context: roslyn, read
public class RoslynDiagnosticsInspector : ICompilationDiagnosticsInspector<CSharpCompilation, Diagnostic>
{
    private readonly IAppLogger<AppLevel> _logger;

    public RoslynDiagnosticsInspector(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: roslyn, read
    public Task<ImmutableArray<Diagnostic>> LogAndFilterDiagnosticsAsync(CSharpCompilation compilation, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Preparing diagnostics", LogLevelNode.Start);
        var diagnostics = compilation.GetDiagnostics(cancellationToken);
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Err, $"Diagnostics Error: {diagnostic}");
            }
            else if (diagnostic.Severity == DiagnosticSeverity.Warning)
            {
                _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Diagnostics Warning: {diagnostic}");
            }
        }
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return Task.FromResult(diagnostics);
    }
}
