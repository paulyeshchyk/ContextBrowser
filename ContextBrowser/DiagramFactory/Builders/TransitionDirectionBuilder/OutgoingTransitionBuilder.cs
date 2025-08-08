using ContextBrowser.DiagramFactory.Renderer.Model;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class OutgoingTransitionBuilder : ITransitionBuilder
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly RoslynCodeParserOptions _options;

    public OutgoingTransitionBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _onWriteLog = onWriteLog;
        _options = options;
    }

    public DiagramDirection Direction => DiagramDirection.Outgoing;


    public GroupedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var resultList = new GroupedTransitionList();
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, "Iterating domain methods", LogLevelNode.Start);
        foreach(var ctx in domainMethods.OrderBy(m => m.SpanStart))
        {
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, $"Getting references for method [{ctx.Name}]", LogLevelNode.Start);
            foreach(var callee in ctx.GetReferences())
            {
                if(callee.ElementType != ContextInfoElementType.method)
                {
                    _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Warn, $"Найдена ссылка, записанная в Reference, но не являющаяся методом [{callee.Name}]");
                    continue;
                }
                var result = UmlTransitionDtoBuilder.CreateTransition(ctx, callee, _onWriteLog, _options);
                if(result != null)
                {
                    resultList.Add((UmlTransitionDto)result);
                }
            }
            _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        }
        _onWriteLog?.Invoke(AppLevel.P_Tran, LogLevel.Dbg, string.Empty, LogLevelNode.End);
        return resultList;
    }
}
