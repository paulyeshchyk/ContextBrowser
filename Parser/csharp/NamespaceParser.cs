namespace ContextBrowser.Parser.csharp;

class NamespaceParser : ILineParser
{
    public bool TryParse(string line, ParseContext ctx)
    {
        var m = line.MatchNamespace();
        if (m?.Success ?? false)
        {
            ctx.Namespace = m.Groups[1].Value.Trim().TrimEnd(';');
            return true;
        }
        return false;
    }
}
