using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Phases.Invocations;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Wrappers.Extractor;

//context roslyn, builder
public class RoslynInvocationSyntaxExtractor : IInvocationSyntaxResolver
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly ISemanticInvocationResolver _semanticInvocationResolver;
    private readonly SemanticOptions _options;

    public RoslynInvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, SemanticOptions options, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
        _options = options;
    }

    // context: roslyn, read
    public BaseSyntaxWrapper? ResolveInvocationSymbol(object invocation, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Resolving symbol for invocation [{invocation}]", LogLevelNode.Start);

        if (invocation is not InvocationExpressionSyntax byInvocation)
        {
            throw new Exception("Invocation is not InvocationExpressionSyntax");
        }

        var result = GetSyntaxWrapper(byInvocation, cancellationToken);
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return result;
    }

    private BaseSyntaxWrapper? GetSyntaxWrapper(InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {

        BaseSyntaxWrapper? result;
        var invocationWrapper = new RoslynInvocationExpressionWrapper(byInvocation, _semanticInvocationResolver);
        var invocationSemanticModel = FindSemanticModel(invocationWrapper);
        if (invocationSemanticModel == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{invocationWrapper.Expression}]");
            result = byInvocation.GetMethodInfoFromSyntax(_options, _onWriteLog);
        }
        else
        {
            cancellationToken.ThrowIfCancellationRequested();
            var symbol = RoslynMethodSymbolExtractor.GetMethodSymbol(invocationWrapper, invocationSemanticModel, _onWriteLog, cancellationToken);

            result = (symbol != null)
                ? RoslynSyntaxWrapperExtractor.Extract(byInvocation, symbol, _onWriteLog)
                : byInvocation.GetMethodInfoFromSyntax(_options, _onWriteLog);
        }
        return result;
    }

    // context: roslyn, read
    internal ISemanticModelWrapper? FindSemanticModel(IInvocationNodeWrapper wrapper)
    {
        var treeWrapper = wrapper.GetTree();
        if (treeWrapper == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{wrapper}]");

            return null;
        }

        return _semanticInvocationResolver.Resolve(treeWrapper);
    }
}
