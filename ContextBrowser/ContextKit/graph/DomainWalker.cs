using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.graph;

// context: build, ContextInfo
// pattern: Visitor
// pattern note: weak
public sealed class DomainWalker : Walker<ContextInfo>
{
    public DomainWalker(string startingDomain, Action<ContextInfo>? visitCallback = default) : base(visitCallback)
    {
        StartingDomain = startingDomain;
    }

    public void Walk(List<ContextInfo> allContexts)
    {
        foreach(var item in allContexts.Where(i => i.Domains.Contains(StartingDomain)))
        {
            AddToVisited(item, Visited);
        }
    }

    public string StartingDomain { get; }
}