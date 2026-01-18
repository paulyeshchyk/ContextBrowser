using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task<IEnumerable<TContext>> LinkAsync(List<InvocationExpressionSyntax> invocationList, TContext callerContext, TContext callerContextInfo, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (invocationList.Count == 0)
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"No invocation to resolve for [{callerContext.FullName}]");
            return [];
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, $"Resolving invocations for [{callerContext.FullName}]", LogLevelNode.Start);

        var result = new List<TContext>();
        foreach (var invocation in invocationList)
        {
            var resolvedItem = await ResolveSymbolThenLinkAsync(invocation, callerContextInfo, _linksInvocationBuilder, options, cancellationToken).ConfigureAwait(false);
            if (resolvedItem != null)
                result.Add(resolvedItem);
        }
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Trace, string.Empty, LogLevelNode.End);
        return result;
    }

    // context: roslyn, invocation, syntax, read
    internal async Task<TContext?> ResolveSymbolThenLinkAsync(InvocationExpressionSyntax invocation, TContext callerContextInfo, IInvocationLinksBuilder<TContext> linksInvocationBuilder, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var symbolDto = await _invocationSyntaxExtractor.ResolveInvocationSymbolAsync(invocation, options, cancellationToken).ConfigureAwait(false);
        if (symbolDto == null)
        {
            return default;
        }

        return await linksInvocationBuilder.LinkInvocationAsync(callerContextInfo, symbolDto, options, cancellationToken).ConfigureAwait(false);
    }
}
