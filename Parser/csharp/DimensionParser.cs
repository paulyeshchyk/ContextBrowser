namespace ContextBrowser.Parser.csharp;

class DimensionParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        var m = line.MatchDimension();
        if(!(m?.Success ?? false))
            return false;

        var key = m.Groups[1].Value.Trim().ToLowerInvariant();
        var value = m.Groups[2].Value.Trim();
        if(key == "context")
            return true;

        // ВСЕГДА сохраняем в pending
        ctx.PendingDimensions[key] = value;
        return true;
    }
}
