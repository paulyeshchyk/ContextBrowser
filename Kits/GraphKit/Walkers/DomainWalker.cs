using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Service;

namespace GraphKit.Walkers;

// context: build, Walker, graph
// pattern: Visitor
// pattern note: weak
public sealed class DomainWalker : Walker<ContextInfo>
{
    public DomainWalker(string startingDomain, IContextInfoManager<ContextInfo> contextInfoManager, Action<ContextInfo>? visitCallback = null) : base(contextInfoManager, visitCallback)
    {
        StartingDomain = startingDomain;
    }

    // context: build, Walker, graph
    public void Walk(List<ContextInfo> allContexts)
    {
        foreach (var item in allContexts.Where(i => i.Domains.Contains(StartingDomain)))
        {
            AddToVisited(item, Visited);
        }
    }

    public string StartingDomain { get; }
}