namespace ContextBrowser.Parser.csharp;

class MethodParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        var m = line.MatchMethod();
        if(!(m?.Success ?? false))
            return false;

        var name = m.Groups[1].Value.Trim();
        var full = $"{ctx.CurrentClass?.Name}.{name}";
        var mi = new ContextInfo
        {
            ElementType = "method",
            Name = full,
            Namespace = ctx.Namespace,
            ClassOwner = ctx.CurrentClass?.Name
        };

        foreach(var kv in ctx.PendingDimensions)
            mi.Dimensions[kv.Key] = kv.Value;

        foreach(var tag in ctx.PendingContexts.Distinct())
            mi.Contexts.Add(tag);

        ctx.PendingDimensions.Clear();
        ctx.PendingContexts.Clear();

        ctx.Result.Add(mi);
        ctx.CurrentMethod = mi;
        ctx.InsideMethod = true;
        return true;
    }
}

