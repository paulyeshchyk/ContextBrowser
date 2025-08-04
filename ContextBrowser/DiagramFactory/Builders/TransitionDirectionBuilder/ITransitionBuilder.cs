using ContextBrowser.DiagramFactory.Model;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}

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
                    continue;
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

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        foreach(var callee in domainMethods)
        {
            foreach(var caller in callee.InvokedBy ?? Enumerable.Empty<ContextInfo>())
            {
                if(caller.ElementType != ContextInfoElementType.method)
                    continue;

                var result = UmlTransitionDtoBuilder.CreateTransition(caller, callee, _onWriteLog, _options);
                if(result != null)
                {
                    yield return (UmlTransitionDto)result;
                }
            }
        }
    }
}

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing;
    private readonly IncomingTransitionBuilder _incoming;

    public BiDirectionalTransitionBuilder(RoslynCodeParserOptions options, OnWriteLog? onWriteLog)
    {
        _outgoing = new OutgoingTransitionBuilder(options, onWriteLog);
        _incoming = new IncomingTransitionBuilder(options, onWriteLog);
    }

    public IEnumerable<UmlTransitionDto> BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        return _outgoing.BuildTransitions(domainMethods, allContexts)
                        .Concat(_incoming.BuildTransitions(domainMethods, allContexts));
    }
}