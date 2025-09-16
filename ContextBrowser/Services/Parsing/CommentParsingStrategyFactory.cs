using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Stategies;
using LoggerKit;

namespace ContextBrowser.Services;

public class CommentParsingStrategyFactory<TContext> : ICommentParsingStrategyFactory<TContext>
    where TContext : ContextInfo
{
    private readonly IAppLogger<AppLevel> _appLogger;

    public CommentParsingStrategyFactory(IAppLogger<AppLevel> appLogger)
    {
        _appLogger = appLogger;
    }

    public IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies(IDomainPerActionContextClassifier classifier)
    {
        return new List<ICommentParsingStrategy<TContext>>()
        {
            new CoverageStrategy<TContext>(),
            new ContextValidationDecorator<TContext>(
                classifier,
                new ContextStrategy<TContext>(classifier),
                _appLogger.WriteLog),
        };
    }
}