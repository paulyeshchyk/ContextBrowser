namespace ContextBrowser.Parser.csharp;

internal static class CSharpParser
{
    public static List<ContextInfo> ParseFile(string filePath)
    {
        var ctx = new ParseContext();
        var parsers = new ILineParser[]
        {
            new NamespaceParser(),
            new ContextCommentParser(),
            new DimensionParser(),
            new ClassParser(),
            new MethodParser(),
            new EndMethodParser()
        };

        foreach(var line in File.ReadAllLines(filePath))
        {
            foreach(var parser in parsers)
                if(parser.TryParse(line, ctx))
                    break;
        }

        return ctx.Result;
    }
}
