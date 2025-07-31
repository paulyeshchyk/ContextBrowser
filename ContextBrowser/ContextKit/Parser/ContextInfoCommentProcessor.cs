using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser.CommentParser;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.ContextKit.Parser;

// context: contextInfo, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private readonly List<ICommentParsingStrategy<T>> _strategies = new();

    public ContextInfoCommentProcessor(IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        _strategies.Add(new ContextStrategy<T>(contextClassifier, onWriteLog));
        _strategies.Add(new CoverageStrategy<T>());
    }

    public void Process(string comment, T target)
    {
        foreach(var strategy in _strategies)
        {
            strategy.Execute(comment, target);
        }
    }
}
