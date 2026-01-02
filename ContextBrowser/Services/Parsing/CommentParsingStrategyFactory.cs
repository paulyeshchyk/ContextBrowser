using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Comment;
using ContextKit.ContextData.Comment.Stategies;
using ContextKit.Model;
using LoggerKit;

namespace ContextBrowser.Services.Parsing;

public class CommentParsingStrategyFactory<TContext> : ICommentParsingStrategyFactory<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAppOptionsStore _optionsStore;

    public CommentParsingStrategyFactory(IAppOptionsStore optionsStore, IAppLogger<AppLevel> logger)
    {
        _logger = logger;
        _optionsStore = optionsStore;
    }

    public IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies()
    {
        return new List<ICommentParsingStrategy<TContext>>()
        {
            new CoverageStrategy<TContext>(),
            new ContextValidationDecorator<TContext>(new ContextStrategy<TContext>(_optionsStore, _logger), _logger),
        };
    }
}