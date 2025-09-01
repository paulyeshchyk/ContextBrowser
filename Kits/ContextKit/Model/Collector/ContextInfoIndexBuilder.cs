using System;
using System.Collections.Generic;
using System.Linq;

namespace ContextKit.Model.Collector;

public static class ContextInfoFullNameIndexBuilder
{
    public static Dictionary<string, TContext> Build<TContext>(IEnumerable<TContext> items)
        where TContext : IContextWithReferences<TContext>
    {
        return items
            .Where(x => !string.IsNullOrWhiteSpace(x.FullName))
            .GroupBy(x => x.FullName)
            .ToDictionary(
                g => g.Key,
                g => g.First(),
                StringComparer.OrdinalIgnoreCase);
    }
}

public static class ContextInfoElementTypeAndNameIndexBuilder
{
    public static Dictionary<string, TContext> Build<TContext>(IEnumerable<TContext> items)
        where TContext : IContextWithReferences<TContext>
    {
        return items
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.NameWithClassOwnerName)
            .ToDictionary(
                g => g.Key,
                g => g.First());
    }
}