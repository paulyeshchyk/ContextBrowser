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
    private OnWriteLog? _onWriteLog;
    private ISemanticInvocationResolver _semanticInvocationResolver;
    private SemanticOptions _options;

    public RoslynInvocationSyntaxExtractor(ISemanticInvocationResolver semanticInvocationResolver, SemanticOptions options, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _semanticInvocationResolver = semanticInvocationResolver;
        _options = options;
    }

    // context: roslyn, read
    public IInvocationSyntaxWrapper? ResolveInvocationSymbol(object invocation, CancellationToken cancellationToken)
    {
#warning do not do checks like this, use wrapper instead
        if (invocation is not InvocationExpressionSyntax byInvocation)
        {
            throw new Exception("Invocation is not InvocationExpressionSyntax");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var invocationWrapper = new RoslynInvocationExpressionWrapper(byInvocation, _semanticInvocationResolver);
        var invocationSemanticModel = FindSemanticModel(invocationWrapper);
        if (invocationSemanticModel == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{invocationWrapper.Expression}]");
#warning use invocationWrapper instead of byInvocation
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        var symbol = RoslynMethodSymbolExtractor.GetMethodSymbol(invocationWrapper, invocationSemanticModel, _onWriteLog, cancellationToken);
        if (symbol == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Symbol was not resolved for invocation [{invocationWrapper.Expression}]");
#warning use invocationWrapper instead of byInvocation
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Resolved symbol [{symbol.GetFullMemberName()}]");

#warning use invocationWrapper instead of byInvocation
        return RoslynSyntaxWrapperExtractor.Extract(byInvocation, symbol, _onWriteLog);
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
