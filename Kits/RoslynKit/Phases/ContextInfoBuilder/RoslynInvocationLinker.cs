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
    private readonly SemanticOptions _options;

    public RoslynInvocationLinker(IInvocationLinksBuilder<TContext> linksInvocationBuilder, OnWriteLog? onWriteLog, IInvocationSyntaxResolver invocationSyntaxExtractor, SemanticOptions options)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _onWriteLog = onWriteLog;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
        _options = options;
    }

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, CancellationToken cancellationToken)
    {
        if (!invocationList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Trace, $"No invocation to resolve for [{callerContext.FullName}]");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Resolving invocations for [{callerContext.FullName}]", LogLevelNode.Start);
        foreach (var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveInvocationSymbol(invocation, cancellationToken);
        if (symbolDto != null)
        {
            linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, _options);
        }
    }
}
