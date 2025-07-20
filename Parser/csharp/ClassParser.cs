namespace ContextBrowser.Parser.csharp;

class ClassParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        var m = line.MatchClass();
        if(!(m?.Success ?? false))
            return false;

        var name = m.Groups[1].Value.Trim();
        var ci = new ContextInfo
        {
            ElementType = "class",
            Name = name,
            Namespace = ctx.Namespace
        };

        // переносим все накопленные dimensions
        foreach(var kv in ctx.PendingDimensions)
            ci.Dimensions[kv.Key] = kv.Value;

        // и tags
        foreach(var tag in ctx.PendingContexts.Distinct())
            ci.Contexts.Add(tag);

        // очищаем накопленные перед следующим элементом
        ctx.PendingDimensions.Clear();
        ctx.PendingContexts.Clear();

        ctx.Result.Add(ci);
        ctx.CurrentClass = ci;
        ctx.CurrentMethod = null;
        ctx.InsideMethod = false;
        return true;
    }
}

