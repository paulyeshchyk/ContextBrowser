using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Extensions;
using ContextKit.Model;
using ContextKit.Model.Service;
using RoslynKit.Model;
using RoslynKit.Parser.Phases;
using RoslynKit.Phases.SymbolLookupHandler;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Phases;

// context: csharp, build
public class RoslynPhaseParserInvocationLinksBuilder<TContext>
    where TContext : ContextInfo, IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly TypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public RoslynPhaseParserInvocationLinksBuilder(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, MethodContextInfoBuilder<TContext> methodContextInfoBuilder, TypeContextInfoBulder<TContext> typeContextInfoBuilder)
    {
        _collector = collector;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _typeContextInfoBuilder = typeContextInfoBuilder;
    }

    // context: csharp, update
    public TContext? LinkInvocation(TContext callerContextInfo, InvocationSyntaxWrapper symbolDto, RoslynCodeParserOptions options)
    {
        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if(calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] Linking invocation \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{symbolDto.FullName}]");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Making links [{callerContextInfo.GetDebugSymbolName()}]", LogLevelNode.Start);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Linking reference: [{callerContextInfo.GetDebugSymbolName()}] Reference [{calleeContextInfo.GetDebugSymbolName()}]");
        var addedReference = ContextInfoService.AddToReference(callerContextInfo, calleeContextInfo);
        if(!addedReference)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL] Linking reference: [{callerContextInfo.GetDebugSymbolName()}] Reference [{calleeContextInfo.GetDebugSymbolName()}]");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Linking invokedBy: [{calleeContextInfo.GetDebugSymbolName()}] InvokedBy [{callerContextInfo.GetDebugSymbolName()}]");
        var addedInvokedBy = calleeContextInfo.InvokedBy.Add(callerContextInfo);
        if(!addedInvokedBy)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"[FAIL] Linking invokedBy: [{callerContextInfo.GetDebugSymbolName()}] InvokedBy [{calleeContextInfo.GetDebugSymbolName()}]");
        }


        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return calleeContextInfo;
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, InvocationSyntaxWrapper symbolDto, RoslynCodeParserOptions options)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Looking for callee by symbol [{symbolDto.FullName}]");

        var fullNameHandler = new FullNameLookupHandler<TContext>(_collector, _onWriteLog);
        var methodSymbolHandler = new MethodSymbolLookupHandler<TContext>(_collector, _onWriteLog);
        var fakeNodeHandler = new FakeNodeCreationHandler<TContext>(_collector, _onWriteLog, options, _typeContextInfoBuilder, _methodContextInfoBuilder);

        // Сначала FullName, затем MethodSymbol, затем FakeNode
        fullNameHandler
            .SetNext(methodSymbolHandler)
            .SetNext(fakeNodeHandler);
        return fullNameHandler.Handle(symbolDto);
    }
}
