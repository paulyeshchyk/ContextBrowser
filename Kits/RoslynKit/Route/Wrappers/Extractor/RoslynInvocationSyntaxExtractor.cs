using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Route.Phases.Invocations;
using RoslynKit.Route.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Route.Wrappers.Extractor;

//context csharp, builder
public class RoslynInvocationSyntaxExtractor
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

    // context: csharp, read
    public IInvocationSyntaxWrapper? ResolveSymbol(InvocationExpressionSyntax byInvocation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocationWrapper = new RoslynInvocationExpressionWrapper(byInvocation, _semanticInvocationResolver);
        var invocationSemanticModel = FindSemanticModel(invocationWrapper);
        if(invocationSemanticModel == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Semantic model was not defined for [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        var symbol = RoslynMethodSymbolExtractor.GetMethodSymbol(invocationWrapper, invocationSemanticModel, _onWriteLog, cancellationToken);
        if(symbol == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[MISS] Symbol was not resolved for invocation [{byInvocation}]");
            return byInvocation.GetMethodInfoFromSyntax(null, _options, _onWriteLog);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[OK] Resolved symbol [{symbol.GetFullMemberName()}]");

        return RoslynSyntaxWrapperExtractor.Extract(byInvocation, symbol, _onWriteLog);
    }

    // context: csharp, read
    internal ISemanticModelWrapper? FindSemanticModel(IInvocationNodeWrapper wrapper)
    {
        var treeWrapper = wrapper.GetTree();
        if(treeWrapper == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: Tree was not provided for invocation [{wrapper}]");

            return null;
        }

        return _semanticInvocationResolver.Resolve(treeWrapper);
    }
}
