using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;
using UmlKit.Infrastructure.Options;

namespace UmlKit.Builders.TransitionDirection;

public class BiDirectionalTransitionBuilder : ITransitionBuilder
{
    public DiagramDirection Direction => DiagramDirection.BiDirectional;

    private readonly OutgoingTransitionBuilder _outgoing;
    private readonly IncomingTransitionBuilder _incoming;

    public BiDirectionalTransitionBuilder(IAppLogger<AppLevel> logger, IContextInfoManager<ContextInfo> _contextInfoManager)
    {
        _outgoing = new OutgoingTransitionBuilder(logger, _contextInfoManager);
        _incoming = new IncomingTransitionBuilder(logger, _contextInfoManager);
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