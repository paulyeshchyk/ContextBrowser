using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class IncomingTransitionBuilder : ITransitionBuilder
{
    private readonly OnWriteLog? _onWriteLog;

    public IncomingTransitionBuilder(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public DiagramDirection Direction => DiagramDirection.Incoming;

    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GrouppedSortedTransitionList();

        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Build incoming", LogLevelNode.Start);

        foreach (var callee in domainMethods)
        {
            BuildCallee(resultList, callee);
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }

    private void BuildCallee(GrouppedSortedTransitionList resultList, ContextInfo callee)
    {
        var invokedByList = callee.InvokedBy;
        if (!invokedByList.Any())
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[SKIP] Building invoked by list for callee {callee.FullName}, no invoked by found");
        }
        var theKey = callee.Identifier;
        foreach (var caller in invokedByList)
        {
            if (caller.ElementType != ContextInfoElementType.method)
            {
                _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"[SKIP] Building invoked by {callee.FullName} -> {caller.FullName}, caller is not method");
                continue;
            }

            var result = UmlTransitionDtoBuilder.CreateTransition(caller, callee, _onWriteLog, theKey);
            if (result == null)
            {
                _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"[MISS] Building invoked by {callee.FullName} -> {caller.FullName}, transition was not created");
                continue;
            }
            resultList.Add((UmlTransitionDto)result, theKey.ToString());
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
