using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using RoslynKit.Assembly.Strategy.Invocation;
using RoslynKit.Lookup;
using RoslynKit.Model.SyntaxWrapper;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Assembly;

// context: roslyn, build
public class RoslynInvocationLinksBuilder : IInvocationLinksBuilder<ContextInfo>
{
    private readonly IContextCollector<ContextInfo> _collector;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ContextInfoBuilderDispatcher<ContextInfo> _contextInfoBuilderDispatcher;
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;
    private readonly ICSharpSyntaxWrapperTypeBuilder _syntaxWrapperTypeBuilder;

    private readonly object _lock = new();

    public RoslynInvocationLinksBuilder(
        IContextCollector<ContextInfo> collector,
        ContextInfoBuilderDispatcher<ContextInfo> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger,
        IContextInfoManager<ContextInfo> contextInfoManager,
        ICSharpSyntaxWrapperTypeBuilder syntaxWrapperTypeBuilder)
    {
        _collector = collector;
        _logger = logger;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
        _contextInfoManager = contextInfoManager;
        _syntaxWrapperTypeBuilder = syntaxWrapperTypeBuilder;
    }

    // context: roslyn, update
    public async Task<ContextInfo?> LinkInvocationAsync(ContextInfo callerContextInfo, ISyntaxWrapper symbolDto, SemanticOptions options)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Linking invocation caller: [{callerContextInfo.FullName}]]", LogLevelNode.Start);
        var calleeContextInfo = await FindOrCreateCalleeNode(symbolDto, options).ConfigureAwait(false);
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

    // context: roslyn, build
    internal void AddInvokedBy(ContextInfo callerContextInfo, ContextInfo calleeContextInfo)
    {
        lock (_lock)
        {
            var addedInvokedBy = _contextInfoManager.AddToInvokedBy(callerContextInfo, calleeContextInfo);

            var message = addedInvokedBy
                ? $"[DONE] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
                : $"[SKIP] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";
            var level = addedInvokedBy
                ? LogLevel.Trace
                : LogLevel.Err;
            _logger.WriteLog(AppLevel.R_Cntx, level, message);
        }
    }

    // context: roslyn, build
    internal void AddReferences(ContextInfo callerContextInfo, ContextInfo calleeContextInfo)
    {
        lock (_lock)
        {
            var addedReference = _contextInfoManager.AddToReferences(callerContextInfo, calleeContextInfo);

            var message = (addedReference)
                ? $"[DONE] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
                : $"[SKIP] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";
            var level = addedReference
                ? LogLevel.Trace
                : LogLevel.Err;
            _logger.WriteLog(AppLevel.R_Cntx, level, message);
        }
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    // context: syntax, read
    internal async Task<ContextInfo?> FindOrCreateCalleeNode(ISyntaxWrapper symbolDto, SemanticOptions options)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"Looking for callee by symbol [{symbolDto.FullName}]");

        var fullNameHandler = new SymbolLookupHandlerFullName<ContextInfo, ISemanticModelWrapper>(_collector, _logger);
        var methodSymbolHandler = new SymbolLookupHandlerMethod<ContextInfo, ISemanticModelWrapper>(_collector, _logger);
        var fakeNodeHandler = new RoslynInvocationLookupHandler<ContextInfo, ISemanticModelWrapper>(_collector, _contextInfoBuilderDispatcher, _logger, options, _syntaxWrapperTypeBuilder);

        // Сначала FullName, затем MethodSymbol, затем FakeNode
        fullNameHandler
            .SetNext(methodSymbolHandler)
            .SetNext(fakeNodeHandler);
        return await fullNameHandler.Handle(symbolDto).ConfigureAwait(false);
    }
}
