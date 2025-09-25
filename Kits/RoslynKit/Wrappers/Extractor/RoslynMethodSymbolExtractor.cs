using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Extractor;

internal static class RoslynMethodSymbolExtractor
{
    // context: roslyn, read
    internal static IMethodSymbol? GetMethodSymbol(IInvocationNodeWrapper invocation, ISemanticModelWrapper semanticModel, IAppLogger<AppLevel> logger, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"Looking IMethodSymbol for expression: {invocation.Expression}");

        var si = semanticModel.GetSymbolInfo(invocation.Expression, cancellationToken);
        if (si is not SymbolInfo symbolInfo)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Exception, $"SymbolInfo not found for {invocation.Expression}");
            return null;
        }

        if (symbolInfo.Symbol is IMethodSymbol method)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[DONE] Found IMethodSymbol for expression: {invocation.Expression}");
            return method;
        }

        if (symbolInfo.CandidateSymbols.Length > 0)
        {
            logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Dbg, $"[DONE] Found Candidate of IMethodSymbol for expression: {invocation.Expression}");
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        }
        logger?.WriteLog(AppLevel.R_Invocation, LogLevel.Warn, $"[FAIL]: No symbol for expression: {invocation.Expression}");
        return null;
    }
}
