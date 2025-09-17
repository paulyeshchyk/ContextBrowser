using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Stategies;
using LoggerKit;

namespace ContextBrowser.Services;

public class CommentParsingStrategyFactory<TContext> : ICommentParsingStrategyFactory<TContext>
    where TContext : ContextInfo
{
    private readonly IAppLogger<AppLevel> _logger;

    public CommentParsingStrategyFactory(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    public IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies(IDomainPerActionContextClassifier classifier)
    {
        return new List<ICommentParsingStrategy<TContext>>()
        {
            new CoverageStrategy<TContext>(),
            new ContextValidationDecorator<TContext>(
                classifier,
                new ContextStrategy<TContext>(classifier, _logger),
                _logger),
        };
    }
}