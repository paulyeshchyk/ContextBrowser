using ContextKit.Model;

namespace GraphKit.Walkers;

// context: build, Walker
// pattern: Visitor
// pattern note: weak
public class Walker<T>
    where T : IContextWithReferences<T>
{
    public readonly HashSet<T> Visited = new();

    public Walker(Action<T>? visitCallback = null)
    {
        VisitCallback = visitCallback;
    }

    protected Action<T>? VisitCallback { get; }

    protected bool CanAddToVisited(T item) => !Visited.Contains(item);

    // context: update, Walker
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