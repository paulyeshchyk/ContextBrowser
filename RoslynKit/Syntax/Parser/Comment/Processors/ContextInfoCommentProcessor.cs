using ContextKit.Model;

namespace RoslynKit.Syntax.Parser.Comment.Processors;

// context: contextInfo, comment, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private readonly List<ICommentParsingStrategy<T>> _strategies = new();

    public ContextInfoCommentProcessor(IEnumerable<ICommentParsingStrategy<T>> strategies)
    {
        _strategies.AddRange(strategies);
    }

    // context: contextInfo, comment, build
    public void Process(string comment, T target)
    {
        foreach(var strategy in _strategies)
        {
            strategy.Execute(comment, target);
        }
    }
}