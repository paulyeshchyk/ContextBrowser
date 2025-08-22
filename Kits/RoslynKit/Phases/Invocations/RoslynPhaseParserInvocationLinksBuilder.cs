using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
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
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
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
    public TContext? LinkInvocation(TContext callerContextInfo, IInvocationSyntaxWrapper symbolDto, SemanticOptions options)
    {
        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if (calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] Linking invocation \ncaller: [{callerContextInfo.FullName}]\nwants:  [{symbolDto.FullName}]");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Making links [{callerContextInfo.GetDebugSymbolName()}]", LogLevelNode.Start);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Linking reference: [{callerContextInfo.GetDebugSymbolName()}] Reference [{calleeContextInfo.GetDebugSymbolName()}]");
        var addedReference = ContextInfoService.AddToReference(callerContextInfo, calleeContextInfo);
        if (!addedReference)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL] Linking reference: [{callerContextInfo.GetDebugSymbolName()}] Reference [{calleeContextInfo.GetDebugSymbolName()}]");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Linking invokedBy: [{calleeContextInfo.GetDebugSymbolName()}] InvokedBy [{callerContextInfo.GetDebugSymbolName()}]");
        var addedInvokedBy = ContextInfoService.AddToInvokedBy(callerContextInfo, calleeContextInfo);
        if (!addedInvokedBy)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL] Linking invokedBy: [{callerContextInfo.GetDebugSymbolName()}] InvokedBy [{calleeContextInfo.GetDebugSymbolName()}]");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return calleeContextInfo;
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, IInvocationSyntaxWrapper symbolDto, SemanticOptions options)
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
