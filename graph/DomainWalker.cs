using ContextBrowser.model;

namespace ContextBrowser.graph;

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