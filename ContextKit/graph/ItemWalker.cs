using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.graph;


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
        if(!AddToVisited(item, Visited))
            return;


        var descendantMethodList = OnGetDescendants.Invoke(item).ToList();
        foreach(var descendantMethod in descendantMethodList)
        {
            if(!AddToVisited(descendantMethod, Visited))
                continue;

            if(skipDescendants)
                continue;
            //OnExportItem(descendantMethod, item, string.Empty);

            var descendantContexts = new HashSet<string>();
            descendantContexts.UnionWith(descendantMethod.Contexts);
            descendantContexts.UnionWith(item.Contexts);

            foreach(var descendantContext in descendantContexts)
            {
                var itemsForDomain = OnGetDomainItems.Invoke(descendantContext).ToList();
                foreach(var itemForDomain in itemsForDomain)
                {
                    OnExportItem(item, descendantMethod, itemForDomain, descendantContext);
                    Walk(itemForDomain, true);
                }
            }
        }
    }
}