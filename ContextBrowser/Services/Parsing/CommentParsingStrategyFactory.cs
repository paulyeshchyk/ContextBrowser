using System.Collections.Generic;
using ContextBrowserKit.Options;
using ContextKit.ContextData;
using ContextKit.ContextData.Comment;
using ContextKit.ContextData.Comment.Stategies;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit;

namespace ContextBrowser.Services.Parsing;

public class CommentParsingStrategyFactory<TContext> : ICommentParsingStrategyFactory<TContext>
    where TContext : ContextInfo
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IAppOptionsStore _optionsStore;
    private readonly ITensorClassifierDomainPerActionContext _classifier;

    public CommentParsingStrategyFactory(IAppOptionsStore optionsStore, IAppLogger<AppLevel> logger)
    {
        _logger = logger;
        _optionsStore = optionsStore;
        _classifier = optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();
    }

    public IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies()
    {
        return new List<ICommentParsingStrategy<TContext>>()
        {
            new CoverageStrategy<TContext>(),
            new ContextValidationDecorator<TContext>(
                _classifier,
                new ContextStrategy<TContext>(_optionsStore, _logger),
                _logger),
        };
    }
}