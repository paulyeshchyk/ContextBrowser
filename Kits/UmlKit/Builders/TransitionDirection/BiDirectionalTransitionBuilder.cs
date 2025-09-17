using System.Collections.Generic;
using ContextBrowserKit.Log;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using UmlKit.Builders.Model;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing;
    private readonly IncomingTransitionBuilder _incoming;

    public BiDirectionalTransitionBuilder(IAppLogger<AppLevel> logger)
    {
        _outgoing = new OutgoingTransitionBuilder(logger);
        _incoming = new IncomingTransitionBuilder(logger);
    }

    public GrouppedSortedTransitionList BuildTransitions(List<ContextInfo> methodsList, List<ContextInfo> allContexts)
    {
        var outgoing = _outgoing.BuildTransitions(methodsList, allContexts);
        var incoming = _incoming.BuildTransitions(methodsList, allContexts);

        var result = new GrouppedSortedTransitionList(outgoing);
        result.Concat(incoming);
        return result;
    }
}