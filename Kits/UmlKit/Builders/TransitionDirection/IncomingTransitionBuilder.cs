using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class IncomingTransitionBuilder : ITransitionBuilder
{
    private readonly IAppLogger<AppLevel> _logger;

    public IncomingTransitionBuilder(IAppLogger<AppLevel> onWriteLog)
    {
        _logger = onWriteLog;
    }

    public DiagramDirection Direction => DiagramDirection.Incoming;

    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GrouppedSortedTransitionList();

        _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"Build incoming", LogLevelNode.Start);

        foreach (var callee in domainMethods)
        {
            BuildCallee(resultList, callee);
        }
        _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }

    private void BuildCallee(GrouppedSortedTransitionList resultList, ContextInfo callee)
    {
        var invokedByList = callee.InvokedBy;
        if (!invokedByList.Any())
        {
            _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"[SKIP] No invoked by found for {callee.FullName}");
            return;
        }

        _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, $"[OK] Invoked found for {callee.FullName}", LogLevelNode.Start);
        var theKey = callee.Identifier;
        foreach (var caller in invokedByList)
        {
            if (caller.ElementType != ContextInfoElementType.method)
            {
                _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Warn, $"[SKIP] Caller is not method {callee.FullName} -> {caller.FullName}");
                continue;
            }

            var result = UmlTransitionDtoBuilder.CreateTransition(caller, callee, _logger, theKey);
            if (result != null)
            {
                resultList.Add((UmlTransitionDto)result, theKey.ToString());
            }
        }
        _logger?.WriteLog(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
