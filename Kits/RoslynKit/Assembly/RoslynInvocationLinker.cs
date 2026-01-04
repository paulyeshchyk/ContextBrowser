using System.Collections.Generic;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

// context: roslyn, invocation, build
public class RoslynInvocationLinker<TContext> : IInvocationLinker<TContext, InvocationExpressionSyntax>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IInvocationLinksBuilder<TContext> _linksInvocationBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IInvocationSyntaxResolver _invocationSyntaxExtractor;

    public RoslynInvocationLinker(IInvocationLinksBuilder<TContext> linksInvocationBuilder, IAppLogger<AppLevel> logger, IInvocationSyntaxResolver invocationSyntaxExtractor)
    {
        _linksInvocationBuilder = linksInvocationBuilder;
        _logger = logger;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
    }

    // context: roslyn, invocation, syntax, build
    public void Link(List<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (invocationList.Count == 0)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"No invocation to resolve for [{callerContext.FullName}]");
            return;
        }

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Resolving invocations for [{callerContext.FullName}]", LogLevelNode.Start);
        foreach (var invocation in invocationList)
        {
            ResolveSymbolThenLink(invocation, callerContextInfo, _linksInvocationBuilder, options, cancellationToken);
        }
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: roslyn, invocation, syntax, read
    internal void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, SemanticOptions options, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveInvocationSymbol(invocation, options, cancellationToken);
        if (symbolDto == null)
        {
            return;
        }

        linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, options);
    }
}
