using ContextKit.Model;
using ContextKit.Model.Service;

namespace GraphKit.Walkers;

// context: build, Walker
// pattern: Visitor
// pattern note: weak
public class Walker<T>
    where T : ContextInfo, IContextWithReferences<T>
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
        if (!visited.Add(item))
            return false;

        VisitCallback?.Invoke(item);
        var references = ContextInfoService.GetReferencesSortedByInvocation(item);
        foreach (var reference in references.OfType<T>())
            AddToVisited(reference, visited);
        return true;
    }
}