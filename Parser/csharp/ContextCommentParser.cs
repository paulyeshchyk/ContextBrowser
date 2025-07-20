namespace ContextBrowser.Parser.csharp;

public class ContextCommentParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        var m = line.MatchContext();
        if(!(m?.Success ?? false))
            return false;

        var tags = m.Groups[1].Value
          .Split(',', ';')
          .Select(t => t.Trim().ToLower());

        foreach(var tag in tags)
        {
            if(ctx.CurrentMethod != null)
                ctx.CurrentMethod.Contexts.Add(tag);
            else if(ctx.CurrentClass != null)
                ctx.CurrentClass.Contexts.Add(tag);
        }

        ctx.PendingContexts.AddRange(tags);
        return true;
    }
}
