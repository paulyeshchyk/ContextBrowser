using ContextBrowser.DiagramFactory.Renderer.Model;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class IncomingTransitionBuilder : ITransitionBuilder
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly RoslynCodeParserOptions _options;

    public IncomingTransitionBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _options = options;
        _onWriteLog = onWriteLog;
    }

    public DiagramDirection Direction => DiagramDirection.Incoming;

    public GroupedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GroupedTransitionList();

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build incoming", LogLevelNode.Start);

        foreach(var callee in domainMethods)
        {
            BuildCallee(resultList, callee);
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }

    private void BuildCallee(GroupedTransitionList resultList, ContextInfo callee)
    {
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build invoked by list for callee {callee.SymbolName}", LogLevelNode.Start);
        var invokedByList = callee.GetInvokedBy();
        if(!invokedByList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[SKIP] Building invoked by list for callee {callee.SymbolName}, no invoked by found");
        }

        foreach(var caller in invokedByList)
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Building invoked by {callee.SymbolName} -> {caller.SymbolName}");
            if(caller.ElementType != ContextInfoElementType.method)
            {
                _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[SKIP] Building invoked by {callee.SymbolName} -> {caller.SymbolName}, caller is not method");
                continue;
            }

            var result = UmlTransitionDtoBuilder.CreateTransition(caller, callee, _onWriteLog, _options);
            if(result == null)
            {
                _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"[MISS] Building invoked by {callee.SymbolName} -> {caller.SymbolName}, transition was not created");
                continue;
            }
            resultList.Add((UmlTransitionDto)result);
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
