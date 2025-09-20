using System;
using System.Collections.Generic;
using ContextKit.Model;
using UmlKit.Builders.Model;

namespace UmlKit.Builders.Strategies;

public class MethodOnlyDiagramBuilder : IContextDiagramBuilder
{
    public GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts, IDomainPerActionContextTensorClassifier classifier)
    {
        throw new NotImplementedException();
    }
}