namespace ContextBrowser.Parser.csharp;

class EndMethodParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        if (ctx.InsideMethod && line.Trim() == "}")
        {
            ctx.CurrentMethod = null;
            ctx.InsideMethod = false;
            return true;
        }
        return false;
    }
}
