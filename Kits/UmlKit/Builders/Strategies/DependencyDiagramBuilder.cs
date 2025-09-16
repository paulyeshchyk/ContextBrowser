using System;
using System.Collections.Generic;
using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders.Strategies;

public class DependencyDiagramBuilder : IContextDiagramBuilder
{
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts, IDomainPerActionContextClassifier classifier)
    {
        throw new NotImplementedException();
    }
}