using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class RoslynInvocationLinker<TContext> : IInvocationLinker<TContext, InvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private IInvocationLinksBuilder<TContext> _linksInvocationBuilder;
    private OnWriteLog? _onWriteLog;
    private IInvocationSyntaxResolver _invocationSyntaxExtractor;
    private SemanticOptions _options;

    public RoslynInvocationLinker(IInvocationLinksBuilder<TContext> linksInvocationBuilder, OnWriteLog? onWriteLog, IInvocationSyntaxResolver invocationSyntaxExtractor, SemanticOptions options)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _onWriteLog = onWriteLog;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
        _options = options;
    }

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Resolving invocations for [{callerContext.GetDebugSymbolName()}]", LogLevelNode.Start);
        foreach (var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveInvocationSymbol(invocation, cancellationToken);
        if (symbolDto == null)
        {
            //TODO: warn
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL]: Symbol was not found [{invocation}]");
            return;
        }

        linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, _options);
    }
}
