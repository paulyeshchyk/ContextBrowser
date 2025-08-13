using ContextKit.Model;

namespace RoslynKit.Basics.Comment.Stategies;

public class CoverageStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "coverage";

    public void Execute(T? container, string comment)
    {
        var content = CommentWithKeywordParser.ExtractContent(Keyword, comment);

        if(string.IsNullOrEmpty(content) || container == null)
        {
            return;
        }

        var val = content.Trim();
        container.Dimensions["coverage"] = val;
    }
}