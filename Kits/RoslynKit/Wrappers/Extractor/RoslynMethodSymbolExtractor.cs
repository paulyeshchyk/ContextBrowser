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

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Looking IMethodSymbol for expression: {invocation.Expression}");

        var si = semanticModel.GetSymbolInfo(invocation.Expression, cancellationToken);
        if (si is not SymbolInfo symbolInfo)
        {
            throw new Exception("si is not SymbolInfo");
        }

        if (symbolInfo.Symbol is IMethodSymbol method)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Found IMethodSymbol for expression: {invocation.Expression}");
            return method;
        }

        if (symbolInfo.CandidateSymbols.Length > 0)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Found Candidate of IMethodSymbol for expression: {invocation.Expression}");
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        }

        // TODO: warn
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS]: No symbol for expression: {invocation.Expression}");
        return null;
    }
}
