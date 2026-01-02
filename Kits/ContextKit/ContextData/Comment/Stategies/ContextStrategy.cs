using System;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit;

namespace ContextKit.ContextData.Comment.Stategies;

// context: contextinfo, build
public class ContextStrategy<TContext> : ICommentParsingStrategy<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public static string Keyword => "context";

    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextClassifier<TContext> _contextClassifier;
    private readonly IFakeDimensionClassifier _fakeDimensionClassifier;

    public ContextStrategy(IAppOptionsStore appOptionsStore, IAppLogger<AppLevel> logger)
    {
        _contextClassifier = appOptionsStore.GetOptions<IContextClassifier<TContext>>();
        _fakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
        _logger = logger;
    }

    // context: contextinfo, build
    public void Execute(TContext? container, string comment)
    {
        var content = CommentWithKeywordParser.ExtractContent(Keyword, comment);

        if (string.IsNullOrEmpty(content))
        {
            return;
        }
        if (container == null)
        {
            _logger.WriteLog(AppLevel.R_Comments, LogLevel.Err, "Comment container is null");
            return;
        }

        var tags = content.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim().ToLowerInvariant())
                          .ToList();

        var actions = tags.Where(t => _contextClassifier.IsVerb(t, _fakeDimensionClassifier)).ToList();
        container.Action = string.Join(";", actions);

        var domains = tags.Where(t => _contextClassifier.IsNoun(t, _fakeDimensionClassifier)).ToList();
        foreach (var domain in domains)
        {
            container.Domains.Add(domain.ToLower());
        }

        foreach (var tag in tags)
        {
            container.Contexts.Add(tag);
        }
    }
}