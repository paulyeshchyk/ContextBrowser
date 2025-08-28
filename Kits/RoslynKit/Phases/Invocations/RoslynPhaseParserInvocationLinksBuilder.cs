using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Lookup;
using RoslynKit.Wrappers.LookupHandler;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Invocations;

// context: roslyn, build
public class RoslynPhaseParserInvocationLinksBuilder<TContext> : IInvocationLinksBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private readonly IContextCollector<TContext> _collector;
    private readonly OnWriteLog? _onWriteLog;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public RoslynPhaseParserInvocationLinksBuilder(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder, CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder)
    {
        _collector = collector;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _typeContextInfoBuilder = typeContextInfoBuilder;
    }

    // context: roslyn, update
    public TContext? LinkInvocation(TContext callerContextInfo, BaseSyntaxWrapper symbolDto, SemanticOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"Linking invocation caller: [{callerContextInfo.FullName}]]", LogLevelNode.Start);
        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if (calleeContextInfo != null)
        {
            AddReferences(callerContextInfo, calleeContextInfo);
            AddInvokedBy(callerContextInfo, calleeContextInfo);
        }

        var message = (calleeContextInfo != null)
            ? $"[DONE] Linking invocation caller: [{callerContextInfo.FullName}] with:  [{symbolDto.FullName}]"
            : $"[FAIL] Linking invocation caller: [{callerContextInfo.FullName}] with:  [{symbolDto.FullName}]";
        _onWriteLog?.Invoke(AppLevel.R_Inv, (calleeContextInfo != null) ? LogLevel.Dbg : LogLevel.Err, message, LogLevelNode.End);

        return calleeContextInfo;
    }

    private void AddInvokedBy(TContext callerContextInfo, TContext calleeContextInfo)
    {
        var addedInvokedBy = ContextInfoService.AddToInvokedBy(callerContextInfo, calleeContextInfo);
        var message = addedInvokedBy
            ? $"[DONE] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
            : $"[SKIP] Adding invokedBy for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";
        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Trace, message);
    }

    private void AddReferences(TContext callerContextInfo, TContext calleeContextInfo)
    {
        var addedReference = ContextInfoService.AddToReference(callerContextInfo, calleeContextInfo);
        var message = (addedReference)
            ? $"[DONE] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]"
            : $"[SKIP] Adding reference for [{callerContextInfo.FullName}] with [{calleeContextInfo.FullName}]";

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Trace, message);
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, BaseSyntaxWrapper symbolDto, SemanticOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Looking for callee by symbol [{symbolDto.FullName}]");

        var fullNameHandler = new SymbolLookupHandlerFullName<TContext, ISemanticModelWrapper>(_collector, _onWriteLog);
        var methodSymbolHandler = new SymbolLookupHandlerMethod<TContext, ISemanticModelWrapper>(_collector, _onWriteLog);
        var fakeNodeHandler = new RoslynInvocationLookupHandler<TContext, ISemanticModelWrapper>(_collector, _onWriteLog, options, _typeContextInfoBuilder, _methodContextInfoBuilder);

        // Сначала FullName, затем MethodSymbol, затем FakeNode
        fullNameHandler
            .SetNext(methodSymbolHandler)
            .SetNext(fakeNodeHandler);
        return fullNameHandler.Handle(symbolDto);
    }
}
