using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Model.Resolver;
using RoslynKit.Syntax.Parser.Wrappers;

namespace RoslynKit.Syntax.Parser.Extractor;

//context csharp, builder
public class CSharpInvocationSyntaxExtractor
{
    private OnWriteLog? _onWriteLog;
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private RoslynCodeParserOptions _options;

    public CSharpInvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
        _options = options;
    }

    // context: csharp, read
    public CSharpInvocationSyntaxWrapper? ResolveSymbol(InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationSemanticModel = FindSemanticModel(byInvocation);
        if(invocationSemanticModel == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        var symbol = SymbolExtractor.GetMethodSymbol(byInvocation, invocationSemanticModel, _onWriteLog, cancellationToken);
        if(symbol == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Symbol was not resolved for invocation [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Resolved symbol [{symbol.GetFullMemberName()}]");

        return SyntaxWrapperExtractor.Extract(byInvocation, symbol, _onWriteLog);
    }

    // context: csharp, read
    internal SemanticModel? FindSemanticModel(InvocationExpressionSyntax invocation)
    {
        var syntaxTree = invocation.SyntaxTree;
        if(syntaxTree == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{invocation}]");

            return null;
        }

        return _semanticInvocationResolver.Resolve(syntaxTree);
    }
}

internal static class SymbolExtractor
{
    // context: csharp, read
    internal static IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax invocation, SemanticModel semanticModel, OnWriteLog? _onWriteLog, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Looking IMethodSymbol for expression: {invocation.Expression}");

        var symbolInfo = semanticModel.GetSymbolInfo(invocation.Expression, cancellationToken);

        if(symbolInfo.Symbol is IMethodSymbol method)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Found IMethodSymbol for expression: {invocation.Expression}");
            return method;
        }

        if(symbolInfo.CandidateSymbols.Length > 0)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Found Candidate of IMethodSymbol for expression: {invocation.Expression}");
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
        }

        // TODO: warn
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS]: No symbol for expression: {invocation.Expression}");
        return null;
    }
}

internal static class SyntaxWrapperExtractor
{
    public static CSharpInvocationSyntaxWrapper? Extract(ExpressionSyntax? resultSyntax, SemanticModel model, OnWriteLog? onWriteLog)
    {
        if(resultSyntax == null)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[ERR]: Syntax not found");
            return default;
        }

        var symbol = model.GetSymbolInfo(resultSyntax).Symbol;
        return Extract(resultSyntax, symbol, onWriteLog);
    }

    public static CSharpInvocationSyntaxWrapper? Extract(ExpressionSyntax resultSyntax, ISymbol? symbol, OnWriteLog? onWriteLog)
    {
        if(symbol == null)
        {
            onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[ERR]: Symbol not resolved for {resultSyntax}");
            return default;
        }

        return new CSharpInvocationSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }
}