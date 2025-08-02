using ContextKit.Model;

namespace GraphKit.Walkers;

// context: build, ContextInfo
// pattern: Visitor
// pattern note: weak
public sealed class ItemWalker : Walker<ContextInfo>
{
    public delegate IEnumerable<ContextInfo> GetDescendantsFunction(ContextInfo context);

    public delegate IEnumerable<ContextInfo> GetDomainItemsFunction(string domain);

    public delegate void ExportItem(ContextInfo item, ContextInfo descendant, ContextInfo? descendantDomainItem, string domain);

    private GetDescendantsFunction OnGetDescendants;
    private GetDomainItemsFunction OnGetDomainItems;
    private ExportItem OnExportItem;

    public ItemWalker(GetDescendantsFunction onGetDescendants, GetDomainItemsFunction onGetDomainItems, ExportItem onExportItem, Action<ContextInfo>? visitCallback = default) : base(visitCallback)
    {
        OnGetDescendants = onGetDescendants;
        OnGetDomainItems = onGetDomainItems;
        OnExportItem = onExportItem;
    }

    // context: build, ContextInfo
    public void Walk(IEnumerable<ContextInfo> items, bool skipDescendants = false)
    {
        foreach(var item in items)
        {
            Walk(item, skipDescendants);
        }
    }

    // context: ContextInfo, read
    public void Walk(ContextInfo item, bool skipDescendants = false)
    {
        // Добавляем item и все его References в Visited
        if(!AddToVisited(item, Visited))
            return;

        var descendants = OnGetDescendants.Invoke(item);
        foreach(var descendant in descendants)
        {
            // специально не проверяем Visited здесь
            AddToVisited(descendant, Visited);

            if(!skipDescendants)
                TryExport(item, descendant);
        }
    }

    private void TryExport(ContextInfo caller, ContextInfo callee)
    {
        var contexts = new HashSet<string>();
        contexts.UnionWith(caller.Contexts);
        contexts.UnionWith(callee.Contexts);

        foreach(var domain in contexts)
        {
            var domainItems = OnGetDomainItems.Invoke(domain);
            foreach(var itemForDomain in domainItems)
            {
                OnExportItem(caller, callee, itemForDomain, domain);

                if(!Visited.Contains(itemForDomain))
                    Walk(itemForDomain, true);
            }
        }
    }
}