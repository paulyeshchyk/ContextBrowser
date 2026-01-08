using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Service;

namespace GraphKit.Walkers;

// context: build, Walker, graph
// pattern: Visitor
// pattern note: weak
public class Walker<T>
    where T : ContextInfo, IContextWithReferences<T>
{
    private readonly IContextInfoManager<ContextInfo> _contextInfoManager;

    public readonly HashSet<T> Visited = new();

    public Walker(IContextInfoManager<ContextInfo> contextInfoManager, Action<T>? visitCallback = null)
    {
        VisitCallback = visitCallback;
        _contextInfoManager = contextInfoManager;
    }

    protected Action<T>? VisitCallback { get; }

    protected bool CanAddToVisited(T item) => !Visited.Contains(item);

    // context: update, Walker, graph
    protected bool AddToVisited(T item, HashSet<T> visited)
    {
        if (!visited.Add(item))
            return false;

        VisitCallback?.Invoke(item);
        var references = _contextInfoManager.GetReferencesSortedByInvocation(item);
        foreach (var reference in references.OfType<T>())
            AddToVisited(reference, visited);
        return true;
    }
}