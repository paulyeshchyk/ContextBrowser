using ContextBrowser.ContextKit.Parser.CommentParser;
using ContextKit.Model;

namespace ContextBrowser.ContextKit.Parser;

// context: contextInfo, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private readonly List<ICommentParsingStrategy<T>> _strategies = new();

    public ContextInfoCommentProcessor(IEnumerable<ICommentParsingStrategy<T>> strategies)
    {
        _strategies.AddRange(strategies);
    }

    public void Process(string comment, T target)
    {
        foreach(var strategy in _strategies)
        {
            strategy.Execute(comment, target);
        }
    }
}