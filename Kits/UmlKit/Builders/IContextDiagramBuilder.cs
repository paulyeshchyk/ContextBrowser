using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContextKit.Model;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;

namespace UmlKit.Builders;

// context: build, transition
public interface IContextDiagramBuilder
{
    Task<GrouppedSortedTransitionList?> BuildAsync(string metaItem, FetchType fetchType, List<ContextInfo> allContexts);
}

public enum FetchType
{
    FetchAction,
    FetchDomain
}