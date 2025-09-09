using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.Invocations;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class RoslynInvocationReferenceBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    protected readonly IContextFactory<TContext> _factory;
    private readonly IInvocationSyntaxResolver _invocationSyntaxResolver;
    private readonly IContextCollector<TContext> _collector;
    private readonly IInvocationLinker<TContext, InvocationExpressionSyntax> _invocationLinker;
    private readonly ISemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax> _referenceBuilderValidator;

    public RoslynInvocationReferenceBuilder(IAppLogger<AppLevel> logger, IContextFactory<TContext> factory, IInvocationSyntaxResolver invocationSyntaxResolver, IContextCollector<TContext> collector)
    {
        _logger = logger;
        _factory = factory;
        _collector = collector;
        _invocationSyntaxResolver = invocationSyntaxResolver;

        var typeContextInfoBuilder = new CSharpTypeContextInfoBulder<TContext>(_collector, _factory, _logger.WriteLog);
        var methodContextInfoBuilder = new CSharpMethodContextInfoBuilder<TContext>(_collector, _factory, _logger.WriteLog);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(_collector, _logger.WriteLog, methodContextInfoBuilder, typeContextInfoBuilder);
        _invocationLinker = new RoslynInvocationLinker<TContext>(linksInvocationBuilder, _logger.WriteLog, _invocationSyntaxResolver);
        _referenceBuilderValidator = new SemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_logger.WriteLog);
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
