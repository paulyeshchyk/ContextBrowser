using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Lookup;
using RoslynKit.Wrappers.LookupHandler;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Phases.Invocations;

// context: roslyn, build
public class RoslynPhaseParserInvocationLinksBuilder<TContext> : IInvocationLinksBuilder<TContext>
    where TContext : class, IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public RoslynPhaseParserInvocationLinksBuilder(
        IContextCollector<TContext> collector,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
    {
        _collector = collector;
        _logger = logger;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    // context: roslyn, update
    public TContext? LinkInvocation(TContext callerContextInfo, ISyntaxWrapper symbolDto, SemanticOptions options)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Linking invocation caller: [{callerContextInfo.FullName}]]", LogLevelNode.Start);
        var calleeContextInfo = FindOrCreateCalleeNode(symbolDto, options);
        if (calleeContextInfo != null)
        {
            AddReferences(callerContextInfo, calleeContextInfo);
            AddInvokedBy(callerContextInfo, calleeContextInfo);
        }
        else
        {
            var message = (calleeContextInfo != null)
                ? $"[DONE] Linking invocation caller: [{callerContextInfo.FullName}] with:  [{symbolDto.FullName}]"
                : $"[FAIL] Linking invocation caller: [{callerContextInfo.FullName}] with:  [{symbolDto.FullName}]";
            _logger.WriteLog(AppLevel.R_Cntx, (calleeContextInfo != null) ? LogLevel.Dbg : LogLevel.Err, message, LogLevelNode.End);
        }

        return calleeContextInfo;
    }

    private void AddInvokedBy(TContext callerContextInfo, TContext calleeContextInfo)
    {
        var addedInvokedBy = ContextInfoService.AddToInvokedBy(callerContextInfo, calleeContextInfo);

        var message = addedInvokedBy
            ? $"[DONE] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
            : $"[SKIP] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";
        var level = addedInvokedBy
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }

    private void AddReferences(TContext callerContextInfo, TContext calleeContextInfo)
    {
        var addedReference = ContextInfoService.AddToReferences(callerContextInfo, calleeContextInfo);

        var message = (addedReference)
            ? $"[DONE] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
            : $"[SKIP] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";
        var level = addedReference
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(ISyntaxWrapper symbolDto, SemanticOptions options)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Looking for callee by symbol [{symbolDto.FullName}]");

        var fullNameHandler = new SymbolLookupHandlerFullName<TContext, ISemanticModelWrapper>(_collector, _logger);
        var methodSymbolHandler = new SymbolLookupHandlerMethod<TContext, ISemanticModelWrapper>(_collector, _logger);
        var fakeNodeHandler = new RoslynInvocationLookupHandler<TContext, ISemanticModelWrapper>(_collector, _contextInfoBuilderDispatcher, _logger, options);

        // Сначала FullName, затем MethodSymbol, затем FakeNode
        fullNameHandler
            .SetNext(methodSymbolHandler)
            .SetNext(fakeNodeHandler);
        return fullNameHandler.Handle(symbolDto);
    }
}
