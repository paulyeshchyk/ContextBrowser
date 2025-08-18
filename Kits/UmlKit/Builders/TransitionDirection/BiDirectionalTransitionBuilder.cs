using ContextBrowserKit.Log;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;


    private readonly OutgoingTransitionBuilder _outgoing;
    private readonly IncomingTransitionBuilder _incoming;

    public BiDirectionalTransitionBuilder(OnWriteLog? onWriteLog)
    {
        _outgoing = new OutgoingTransitionBuilder(onWriteLog);
        _incoming = new IncomingTransitionBuilder(onWriteLog);
    }

    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts)
    {
        var outgoing = _outgoing.BuildTransitions(domainMethods, allContexts);
        var incoming = _incoming.BuildTransitions(domainMethods, allContexts);

        var result = new GrouppedSortedTransitionList(outgoing);
        result.Concat(incoming);
        return result;
    }
}