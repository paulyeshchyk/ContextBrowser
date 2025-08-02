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
    public void LinkInvocation(TContext callerContextInfo, string? calleeSymbolName, string? calleeShortestName, RoslynCodeParserOptions options)
    {
        if(string.IsNullOrWhiteSpace(calleeSymbolName) || string.IsNullOrWhiteSpace(calleeShortestName))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] Callee symbol name is empty");
            return;
        }

        var calleeContextInfo = FindOrCreateCalleeNode(callerContextInfo, calleeSymbolName, calleeShortestName, options);
        if(calleeContextInfo == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS] \ncaller: [{callerContextInfo.SymbolName}]\nwants:  [{calleeSymbolName}]");
            return;
        }
        callerContextInfo.References.Add(calleeContextInfo);
        calleeContextInfo.InvokedBy.Add(callerContextInfo);
    }

    private TContext? FindOrCreateCalleeNode(TContext callerContextInfo, string calleeSymbolName, string calleeShortestName, RoslynCodeParserOptions options)
    {
        if(!_collector.ByFullName.TryGetValue(calleeSymbolName, out var calleeContextInfo))
        {
            if(!options.CreateFailedCallees)
            {
                return default;
            }

            //
            var methodmodel = new MethodSyntaxModel() { methodName = calleeShortestName, methodFullName = calleeSymbolName };
            return _methodContextInfoBuilder.BuildContextInfoForMethod(methodmodel, "FakeNamespace", callerContextInfo, null);
        }
        return calleeContextInfo;
    }
}
