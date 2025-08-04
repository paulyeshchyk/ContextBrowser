using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Parser.Phases;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Parser.Extractor;

//context csharp, builder
public class UndefinedInvocationResolver
{
    private OnWriteLog? _onWriteLog;
    private ISemanticInvocationResolver _semanticInvocationResolver;

    public UndefinedInvocationResolver(ISemanticInvocationResolver semanticInvocationResolver, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
    }

    // context: csharp, read
    public RoslynCalleeSymbolDto ResolveSymbol(InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationSemanticModel = FindSemanticModel(byInvocation);
        if(invocationSemanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Semantic model was not defined for {byInvocation}");
            return byInvocation.GetMethodInfoFromSyntax();
        }

        var symbol = GetMethodSymbol(byInvocation, invocationSemanticModel, cancellationToken);
        if(symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Symbol was not resolved for invocation {byInvocation}");
            return byInvocation.GetMethodInfoFromSyntax();
        }
        return symbol.BuildDto(byInvocation.Span.Start, byInvocation.Span.End);
    }

    // context: csharp, read
    internal SemanticModel? FindSemanticModel(InvocationExpressionSyntax invocation)
    {
        var syntaxTree = invocation.SyntaxTree;
        if(syntaxTree == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Tree was not provided for invocation {invocation}");

            return null;
        }

        return _semanticInvocationResolver.Resolve(syntaxTree);
    }

    // context: csharp, read
    internal IMethodSymbol? GetMethodSymbol(InvocationExpressionSyntax byInvocation, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var symbolInfo = semanticModel.GetSymbolInfo(byInvocation, cancellationToken);
        if(symbolInfo.Symbol == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Info, $"No SymbolInfo was found for {byInvocation}");
            return null;
        }

        if(symbolInfo.Symbol is not IMethodSymbol result)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"SymbolInfo was found for {byInvocation}, but it has no MethodSymbol");
            return null;
        }

        return result;
    }
}