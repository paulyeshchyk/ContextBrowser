using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class RoslynInvocationLinker<TContext> : IInvocationLinker<TContext, InvocationExpressionSyntax>
    where TContext : ContextInfo, IContextWithReferences<TContext>
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

    public void Link(IEnumerable<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (!invocationList.Any())
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

    private void ResolveSymbolThenLink(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, SemanticOptions options, CancellationToken cancellationToken)
    {
        var symbolDto = _invocationSyntaxExtractor.ResolveInvocationSymbol(invocation, options, cancellationToken);
        if (symbolDto != null)
        {
            linksInvocationBuilder.LinkInvocation(callerContextInfo, symbolDto, options);
        }
    }
}
