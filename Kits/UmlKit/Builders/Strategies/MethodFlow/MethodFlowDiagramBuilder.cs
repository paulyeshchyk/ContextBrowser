using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;

namespace UmlKit.Builders.Strategies;

public class MethodFlowDiagramBuilder : IContextDiagramBuilder
{
    public Task<GrouppedSortedTransitionList?> BuildAsync(string metaItem, FetchType fetchType, List<ContextInfo> allContexts)
    {
        throw new NotImplementedException();
    }
}