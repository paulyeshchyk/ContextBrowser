using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Extractor;

internal static class RoslynMethodSymbolExtractor
{
    // context: roslyn, read
    internal static IMethodSymbol? GetMethodSymbol(IInvocationNodeWrapper invocation, ISemanticModelWrapper semanticModel, OnWriteLog? _onWriteLog, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Looking IMethodSymbol for expression: {invocation.Expression}");

        var si = semanticModel.GetSymbolInfo(invocation.Expression, cancellationToken);
        if (si is not SymbolInfo symbolInfo)
        {
            throw new Exception("si is not SymbolInfo");
        }

        if (symbolInfo.Symbol is IMethodSymbol method)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"[DONE] Found IMethodSymbol for expression: {invocation.Expression}");
            return method;
        }

        if (symbolInfo.CandidateSymbols.Length > 0)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"[DONE] Found Candidate of IMethodSymbol for expression: {invocation.Expression}");
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        }
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Warn, $"[FAIL]: No symbol for expression: {invocation.Expression}");
        return null;
    }
}
