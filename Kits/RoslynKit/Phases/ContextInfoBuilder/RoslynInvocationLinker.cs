using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class RoslynInvocationLinker<TContext> : IInvocationLinker<TContext, InvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private readonly IInvocationLinksBuilder<TContext> _linksInvocationBuilder;
    private readonly OnWriteLog? _onWriteLog;
    private readonly IInvocationSyntaxResolver _invocationSyntaxExtractor;

    public RoslynInvocationLinker(IInvocationLinksBuilder<TContext> linksInvocationBuilder, OnWriteLog? onWriteLog, IInvocationSyntaxResolver invocationSyntaxExtractor)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _onWriteLog = onWriteLog;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
    }

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (!invocationList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Trace, $"No invocation to resolve for [{callerContext.FullName}]");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, $"Resolving invocations for [{callerContext.FullName}]", LogLevelNode.Start);
        foreach (var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, options, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, SemanticOptions options, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveInvocationSymbol(invocation, options, cancellationToken);
        if (symbolDto != null)
        {
            linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, options);
        }
    }
}
