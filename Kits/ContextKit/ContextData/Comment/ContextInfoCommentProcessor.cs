using System.Collections.Generic;
using ContextKit.Model;

namespace ContextKit.Stategies;

// context: contextInfo, comment, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private readonly List<ICommentParsingStrategy<T>> _strategies = new();

    public ContextInfoCommentProcessor(ICommentParsingStrategyFactory<T> factory, IDomainPerActionContextClassifierBuilder contextClassifierBuilder)
    {
        var contextClassifier = contextClassifierBuilder.Build();
        _strategies.AddRange(factory.CreateStrategies(contextClassifier));
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
