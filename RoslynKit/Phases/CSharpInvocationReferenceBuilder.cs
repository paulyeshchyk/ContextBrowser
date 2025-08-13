using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Model;
using RoslynKit.Semantic.Builder;
using RoslynKit.Syntax.Parser.Extractor;

namespace RoslynKit.Phases;

public class CSharpInvocationReferenceBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private OnWriteLog? _onWriteLog;
    protected readonly IContextFactory<TContext> _factory;
    private CSharpInvocationSyntaxExtractor _invocationSyntaxExtractor;
    private RoslynCodeParserOptions _options;
    private IContextCollector<TContext> _collector;

    public CSharpInvocationReferenceBuilder(OnWriteLog? onWriteLog, IContextFactory<TContext> factory, CSharpInvocationSyntaxExtractor invocationSyntaxExtractor, RoslynCodeParserOptions options, IContextCollector<TContext> collector)
    {
        _onWriteLog = onWriteLog;
        _factory = factory;
        _options = options;
        _collector = collector;
        _invocationSyntaxExtractor = invocationSyntaxExtractor;
    }

    // context: csharp, read
    public void BuildReferences(TContext callerContext, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Build references for [{callerContext.GetDebugName()}]");

        var validator = new ReferenceBuilderValidator<TContext, InvocationExpressionSyntax>(_onWriteLog);

        var validationResult = validator.Validate(callerContext, _collector);
        if(validationResult == null)
        {
            return;
        }

        BuildInvocations(callerContext, validationResult, cancellationToken);
    }

    private void BuildInvocations(TContext callerContext, ReferenceBuilderValidator<TContext, InvocationExpressionSyntax>.ValidationResult validationResult, CancellationToken cancellationToken)
    {
        var callerContextInfo = validationResult.CallerContextInfo;
        var invocationList = validationResult.Invocations;

        var typeContextInfoBuilder = new CSharpTypeContextInfoBulder<TContext>(_collector, _factory, _onWriteLog);
        var methodContextInfoBuilder = new CSharpMethodContextInfoBuilder<TContext>(_collector, _factory, _onWriteLog);

        var linksInvocationBuilder = new RoslynPhaseParserInvocationLinksBuilder<TContext>(_collector, _onWriteLog, methodContextInfoBuilder, typeContextInfoBuilder);
        var invocationLinker = new CSharpInvocationLinker<TContext>(linksInvocationBuilder, _onWriteLog, _invocationSyntaxExtractor, _options);

        invocationLinker.Link(invocationList, callerContext, callerContextInfo, cancellationToken);
    }
}
