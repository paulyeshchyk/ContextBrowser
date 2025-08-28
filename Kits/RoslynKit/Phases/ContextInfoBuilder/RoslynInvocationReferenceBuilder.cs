using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Phases.Invocations;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.ContextInfoBuilder;

public class RoslynInvocationReferenceBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private readonly OnWriteLog? _onWriteLog;
    protected readonly IContextFactory<TContext> _factory;
    private readonly IInvocationSyntaxResolver _invocationSyntaxResolver;
    private readonly SemanticOptions _options;
    private readonly IContextCollector<TContext> _collector;
    private readonly IInvocationLinker<TContext, InvocationExpressionSyntax> _invocationLinker;
    private readonly ISemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax> _referenceBuilderValidator;

    public RoslynInvocationReferenceBuilder(OnWriteLog? onWriteLog, IContextFactory<TContext> factory, IInvocationSyntaxResolver invocationSyntaxResolver, SemanticOptions options, IContextCollector<TContext> collector)
    {
        _onWriteLog = onWriteLog;
        _factory = factory;
        _options = options;
        _collector = collector;
        _invocationSyntaxResolver = invocationSyntaxResolver;

        var typeContextInfoBuilder = new CSharpTypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new CSharpMethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);
        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(_collector, _onWriteLog, methodContextInfoBuilder, typeContextInfoBuilder);
        _invocationLinker = new RoslynInvocationLinker<TContext>(linksInvocationBuilder, _onWriteLog, _invocationSyntaxResolver, _options);
        _referenceBuilderValidator = new SemanticReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_onWriteLog);
    }

    // context: roslyn, read
    public void BuildReferences(TContext callerContext, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Building references for {callerContext.FullName}", LogLevelNode.Start);

        var validationResult = _referenceBuilderValidator.Validate(callerContext, _collector);
        if (validationResult != null)
        {
            var callerContextInfo = validationResult.CallerContextInfo;
            var invocationList = validationResult.Invocations;

            _invocationLinker.Link(invocationList, callerContext, callerContextInfo, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

}
