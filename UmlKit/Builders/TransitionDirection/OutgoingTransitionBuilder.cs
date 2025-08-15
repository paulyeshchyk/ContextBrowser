using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class OutgoingTransitionBuilder : ITransitionBuilder
{
    private readonly OnWriteLog? _onWriteLog;

    public OutgoingTransitionBuilder(OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
    }

    public DiagramDirection Direction => DiagramDirection.Outgoing;


    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GrouppedSortedTransitionList();
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, "Iterating domain methods", LogLevelNode.Start);
        foreach(var ctx in domainMethods.OrderBy(m => m.SpanStart))
        {
            var theKey = ctx.Identifier;

            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Getting references for method [{ctx.Name}]", LogLevelNode.Start);
            var references = ContextInfoService.GetReferencesSortedByInvocation(ctx);
            foreach(var callee in references)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                {
                    _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"Найдена ссылка, записанная в Reference, но не являющаяся методом [{callee.Name}]");
                    continue;
                }
                var result = UmlTransitionDtoBuilder.CreateTransition(ctx, callee, _onWriteLog, theKey);
                if(result == null)
                {
                    _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Err, "Объект UmlTransitionDto не создан");
                    continue;
                }

                resultList.Add(result, theKey.ToString());
            }
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }
}
