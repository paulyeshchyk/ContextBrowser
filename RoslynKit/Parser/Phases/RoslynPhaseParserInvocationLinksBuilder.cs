using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using RoslynKit.Parser.Extractor;

namespace RoslynKit.Parser.Phases;

// context: csharp, build
public class RoslynPhaseParserInvocationLinksBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private IContextCollector<TContext> _collector;
    private OnWriteLog? _onWriteLog;
    private readonly MethodContextInfoBuilder<TContext> _methodContextInfoBuilder;

    public RoslynPhaseParserInvocationLinksBuilder(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, MethodContextInfoBuilder<TContext> methodContextInfoBuilder)
    {
        _collector = collector;
        _onWriteLog = onWriteLog;
        _methodContextInfoBuilder = methodContextInfoBuilder;
    }

    // context: csharp, update
    public TContext? LinkInvocation(TContext callerContextInfo, RoslynCalleeSymbolDto symbolDto, RoslynCodeParserOptions options)
    {
        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, symbolDto, options);
        if(calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{symbolDto.Name}]");
            return default;
        }
        callerContextInfo.References.Add(calleeContextInfo);
        calleeContextInfo.InvokedBy.Add(callerContextInfo);
        return calleeContextInfo;
    }

    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, RoslynCalleeSymbolDto symbolDto, RoslynCodeParserOptions options)
    {
        if(!_collector.ByFullName.TryGetValue(symbolDto.Name, out var calleeContextInfo))
        {
            if(!options.CreateFailedCallees)
            {
                return default;
            }

            //
            var methodmodel = new MethodSyntaxModel() { methodName = symbolDto.ShortName, methodFullName = symbolDto.Name, spanStart = symbolDto.SpanStart, spanEnd = symbolDto.SpanEnd };
            return _methodContextInfoBuilder.BuildContextInfoForMethod(methodmodel, "FakeNamespace", callerContextInfo, null);
        }
        return calleeContextInfo;
    }
}

public record struct RoslynCalleeSymbolDto
{
    public bool isPartial { get; init; }

    public string Name { get; init; }

    public string ShortName { get; init; }

    public int SpanStart { get; init; }

    public int SpanEnd { get; init; }
}
