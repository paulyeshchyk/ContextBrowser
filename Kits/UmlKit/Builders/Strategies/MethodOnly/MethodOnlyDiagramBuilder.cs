using System;
using System.Collections.Generic;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;

namespace UmlKit.Builders.Strategies;

public class MethodOnlyDiagramBuilder : IContextDiagramBuilder
{
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts)
    {
        throw new NotImplementedException();
    }
}