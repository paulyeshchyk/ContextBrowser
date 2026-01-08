using System;
using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Service;

namespace GraphKit.Walkers;

// context: build, Walker, graph
// pattern: Visitor
// pattern note: weak
public sealed class ItemWalker : Walker<ContextInfo>
{
    public delegate IEnumerable<ContextInfo> GetDescendantsFunction(ContextInfo context);

    public delegate IEnumerable<ContextInfo> GetDomainItemsFunction(string domain);

    public delegate void ExportItem(ContextInfo item, ContextInfo descendant, ContextInfo? descendantDomainItem, string domain);

    private readonly GetDescendantsFunction _onGetDescendants;
    private readonly GetDomainItemsFunction _onGetDomainItems;
    private readonly ExportItem _exportItem;

    public ItemWalker(GetDescendantsFunction onGetDescendants, GetDomainItemsFunction onGetDomainItems, ExportItem exportItem, IContextInfoManager<ContextInfo> contextInfoManager, Action<ContextInfo>? visitCallback = null) : base(contextInfoManager, visitCallback)
    {
        _onGetDescendants = onGetDescendants;
        _onGetDomainItems = onGetDomainItems;
        _exportItem = exportItem;
    }

    // context: build, Walker, graph
    public void Walk(IEnumerable<ContextInfo> items, bool skipDescendants = false)
    {
        foreach (var item in items)
        {
            Walk(item, skipDescendants);
        }
    }

    // context: build, Walker, graph
    public void Walk(ContextInfo item, bool skipDescendants = false)
    {
        // Добавляем item и все его References в Visited
        if (!AddToVisited(item, Visited))
            return;

        var descendants = _onGetDescendants.Invoke(item);
        foreach (var descendant in descendants)
        {
            // специально не проверяем Visited здесь
            AddToVisited(descendant, Visited);

            if (!skipDescendants)
                TryExport(item, descendant);
        }
    }

    // context: build, share, Walker, graph
    internal void TryExport(ContextInfo caller, ContextInfo callee)
    {
        var contexts = new HashSet<string>();
        contexts.UnionWith(caller.Contexts);
        contexts.UnionWith(callee.Contexts);

        foreach (var domain in contexts)
        {
            var domainItems = _onGetDomainItems.Invoke(domain);
            foreach (var itemForDomain in domainItems)
            {
                _exportItem(caller, callee, itemForDomain, domain);

                if (!Visited.Contains(itemForDomain))
                    Walk(itemForDomain, true);
            }
        }
    }
}