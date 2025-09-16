using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace ContextKit.Stategies;

// context: contextInfo, comment, build
public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private readonly IAppOptionsStore _optionsStore;
    private readonly ICommentParsingStrategyFactory<T> _factory;

    private readonly List<ICommentParsingStrategy<T>> _strategies;

    public ContextInfoCommentProcessor(ICommentParsingStrategyFactory<T> factory, IAppOptionsStore optionsStore)
    {
        _factory = factory;
        _optionsStore = optionsStore;

        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();
        _strategies = _factory.CreateStrategies(contextClassifier).ToList();
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
