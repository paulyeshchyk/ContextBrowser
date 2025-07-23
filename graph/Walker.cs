using ContextBrowser.model;

namespace ContextBrowser.graph;

public class Walker<T>
    where T : IContextWithReferences<T>
{
    public readonly HashSet<T> Visited = new();

    public Walker(Action<T>? visitCallback = null)
    {
        VisitCallback = visitCallback;
    }

    protected Action<T>? VisitCallback { get; }

    protected bool CanAddToVisited(T item) => !(Visited.Contains(item));

    protected bool AddToVisited(T item, HashSet<T> visited)
    {
        if(!visited.Add(item))
            return false;

        VisitCallback?.Invoke(item);

        foreach(var reference in item.References)
            AddToVisited(reference, visited);
        return true;
    }
}