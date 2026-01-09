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
using SemanticKit.Parsers.Strategy.Invocation;

namespace RoslynKit.Assembly.Strategy.Invocation;

// context: roslyn, read

public class RoslynInvocationBuilder<TContext> : IInvocationBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    protected readonly IContextFactory<TContext> _factory;
    private readonly IContextCollector<TContext> _collector;
    private readonly IInvocationLinker<TContext, InvocationExpressionSyntax> _invocationLinker;
    private readonly IInvocationBuilderValidator<TContext, InvocationExpressionSyntax> _invocationBuilderValidator;

    public RoslynInvocationBuilder(
        IAppLogger<AppLevel> logger,
        IContextFactory<TContext> factory,
        IContextCollector<TContext> collector,
        IInvocationLinker<TContext, InvocationExpressionSyntax> invocationLinker)
    {
        _logger = logger;
        _factory = factory;
        _collector = collector;
        _invocationLinker = invocationLinker;

        _invocationBuilderValidator = new InvocationBuilderValidator<TContext, InvocationExpressionSyntax>(_logger);
    }

    // context: roslyn, read
    public async Task BuildReferencesAsync(TContext callerContext, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Building references for {callerContext.FullName}", LogLevelNode.Start);

        var validationResult = _invocationBuilderValidator.Validate(callerContext, _collector);
        if (validationResult != null)
        {
            var callerContextInfo = validationResult.CallerContextInfo;
            var invocationList = validationResult.Invocations.ToList();

            var linkedItems = await _invocationLinker.LinkAsync(invocationList, callerContext, callerContextInfo, options, cancellationToken).ConfigureAwait(false);
            foreach (var item in linkedItems)
            {
                _collector.Append(item);
            }
        }
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
