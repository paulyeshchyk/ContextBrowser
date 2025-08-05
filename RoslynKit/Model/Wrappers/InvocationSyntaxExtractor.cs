using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Parser.Phases;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Model.Wrappers;

//context csharp, builder
public class InvocationSyntaxExtractor
{
    private OnWriteLog? _onWriteLog;
    private ISemanticInvocationResolver _semanticInvocationResolver;

    public InvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
    }

    public InvocationSyntaxWrapper? Extract(InvocationExpressionSyntax? resultSyntax, SemanticModel model)
    {
        if(resultSyntax == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[ERR] Syntax not found");
            return default;
        }

        var symbol = model.GetSymbolInfo(resultSyntax).Symbol;
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[ERR] Symbol not resolved for {resultSyntax}");
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
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Semantic model was not defined for [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _onWriteLog);
        }

        var symbol = GetMethodSymbol(byInvocation, invocationSemanticModel, cancellationToken);
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"Symbol was not resolved for invocation [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Symbol was resolved for invocation [{byInvocation}]");

        return Extract(byInvocation, invocationSemanticModel);
    }

    // context: csharp, read
    internal SemanticModel? FindSemanticModel(InvocationExpressionSyntax invocation)
    {
        var syntaxTree = invocation.SyntaxTree;
        if(syntaxTree == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Tree was not provided for invocation [{invocation}]");

            return null;
        }

        return _semanticInvocationResolver.Resolve(syntaxTree);
    }

    // context: csharp, read
    internal IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var symbolInfo = semanticModel.GetSymbolInfo(invocation.Expression, cancellationToken);

        if(symbolInfo.Symbol is IMethodSymbol method)
            return method;

        if(symbolInfo.CandidateSymbols.Length > 0)
            return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[FAIL] No symbol for expression: {invocation.Expression}");
        return null;
    }
}