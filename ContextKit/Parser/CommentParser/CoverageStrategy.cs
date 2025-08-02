using ContextKit.Model;

namespace ContextBrowser.ContextKit.Parser.CommentParser;

public class CoverageStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "coverage";

    public void Execute(string comment, T container)
    {
        var content = CommentWithKeywordParser.ExtractContent(Keyword, comment);

        if(string.IsNullOrEmpty(content))
        {
            return;
        }

        var val = content.Trim();
        container.Dimensions["coverage"] = val;
    }
}