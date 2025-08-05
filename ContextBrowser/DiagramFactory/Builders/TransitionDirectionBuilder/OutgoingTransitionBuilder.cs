using ContextBrowser.DiagramFactory.Model;
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


    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, "Iterating domain methods", LogLevelNode.Start);
        foreach(var ctx in domainMethods.OrderBy(m => m.SpanStart))
        {
            _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, $"Domain method [{ctx.Name}]");
            foreach(var callee in ctx.References)
            {
                if(callee.ElementType != ContextInfoElementType.method)
                {
                    _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Warn, $"Найдена ссылка, записанная в Reference, но не являющаяся методом [{callee.Name}]");
                    continue;
                }
                var result = UmlTransitionDtoBuilder.CreateTransition(ctx, callee, _onWriteLog, _options);
                if(result != null)
                {
                    yield return (UmlTransitionDto)result;
                }
            }
        }
        _onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
