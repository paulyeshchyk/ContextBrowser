using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Model.Resolver;
using RoslynKit.Parser.Phases;

namespace RoslynKit.Model.Wrappers;

//context csharp, builder
public class InvocationSyntaxExtractor
{
    private OnWriteLog? _onWriteLog;
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private RoslynCodeParserOptions _options;

    public InvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
        _options = options;
    }

    public InvocationSyntaxWrapper? Extract(InvocationExpressionSyntax? resultSyntax, SemanticModel model)
    {
        if(resultSyntax == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[ERR]: Syntax not found");
            return default;
        }

        var symbol = model.GetSymbolInfo(resultSyntax).Symbol;
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"[ERR]: Symbol not resolved for {resultSyntax}");
            return default;
        }

        return new InvocationSyntaxWrapper(symbol: symbol, syntax: resultSyntax);
    }


    // context: csharp, read
    public InvocationSyntaxWrapper? ResolveSymbol(InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationSemanticModel = FindSemanticModel(byInvocation);
        if(invocationSemanticModel == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        var symbol = GetMethodSymbol(byInvocation, invocationSemanticModel, cancellationToken);
        if(symbol == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Symbol was not resolved for invocation [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Resolved symbol [{symbol.GetFullMemberName()}]");

        return Extract(byInvocation, invocationSemanticModel);
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

    // context: csharp, read
    internal IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
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