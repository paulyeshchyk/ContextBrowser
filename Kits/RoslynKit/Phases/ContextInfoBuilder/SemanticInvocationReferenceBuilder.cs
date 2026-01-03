using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

// context: roslyn, read
public class SemanticInvocationReferenceBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    protected readonly IContextFactory<TContext> _factory;
    private readonly IContextCollector<TContext> _collector;
    private readonly IInvocationLinker<TContext, InvocationExpressionSyntax> _invocationLinker;
    private readonly ISemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax> _referenceBuilderValidator;

    public SemanticInvocationReferenceBuilder(
        IAppLogger<AppLevel> logger,
        IContextFactory<TContext> factory,
        IContextCollector<TContext> collector,
        IInvocationLinker<TContext, InvocationExpressionSyntax> invocationLinker)
    {
        _logger = logger;
        _factory = factory;
        _collector = collector;
        _invocationLinker = invocationLinker;

        _referenceBuilderValidator = new SemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_logger);
    }

    // context: roslyn, read
    public void BuildReferences(TContext callerContext, SemanticOptions options, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Building references for {callerContext.FullName}", LogLevelNode.Start);

        var validationResult = _referenceBuilderValidator.Validate(callerContext, _collector);
        if (validationResult != null)
        {
            var callerContextInfo = validationResult.CallerContextInfo;
            var invocationList = validationResult.Invocations.ToList();

            _invocationLinker.Link(invocationList, callerContext, callerContextInfo, options, cancellationToken);
        }
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
