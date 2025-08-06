using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using RoslynKit.Parser.Phases;
using RoslynKit.Phases.SymbolLookupHandler;
using RoslynKit.Syntax.Parser.ContextInfo;

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
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Building invocation [{callerContextInfo.Name}]");

        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if(calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{symbolDto.FullName}]");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Adding invocation Reference [{calleeContextInfo.Name}]");
        callerContextInfo.References.Add(calleeContextInfo);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Adding invocation InvokedBy [{callerContextInfo.Name}]");
        calleeContextInfo.InvokedBy.Add(callerContextInfo);

        return calleeContextInfo;
    }

    // Класс: RoslynPhaseParserInvocationLinksBuilder<TContext>
    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, InvocationSyntaxWrapper symbolDto, RoslynCodeParserOptions options)
    {
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
