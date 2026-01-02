using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;

namespace ContextKit.ContextData.Comment;

// context: contextInfo, comment, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : IContextWithReferences<T>
{
    private readonly List<ICommentParsingStrategy<T>> _strategies;

    public ContextInfoCommentProcessor(ICommentParsingStrategyFactory<T> factory)
    {
        _strategies = factory.CreateStrategies().ToList();
    }

    // context: contextInfo, comment, build
    public void Process(T? target, string comment)
    {
        foreach (var strategy in _strategies)
        {
            strategy.Execute(target, comment);
        }
    }
}
