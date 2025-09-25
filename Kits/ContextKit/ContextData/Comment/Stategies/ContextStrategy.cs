using System;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit;

namespace ContextKit.ContextData.Comment.Stategies;

public class ContextStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "context";

    private readonly IAppLogger<AppLevel> _logger;
    private readonly IWordRoleClassifier _wordRoleClassifier;
    private readonly IFakeDimensionClassifier _fakeDimensionClassifier;

    public ContextStrategy(IAppOptionsStore appOptionsStore, IAppLogger<AppLevel> logger)
    {
        _wordRoleClassifier = appOptionsStore.GetOptions<IWordRoleClassifier>();
        _fakeDimensionClassifier = appOptionsStore.GetOptions<IFakeDimensionClassifier>();
        _logger = logger;
    }

    public void Execute(T? container, string comment)
    {
        var content = CommentWithKeywordParser.ExtractContent(Keyword, comment);

        if (string.IsNullOrEmpty(content))
        {
            return;
        }
        if (!(container != null && container is not null))
        {
            _logger.WriteLog(AppLevel.R_Comments, LogLevel.Err, "Comment container is null");
            return;
        }

        var tags = content.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim().ToLowerInvariant());

        var actions = tags.Where(t => _wordRoleClassifier.IsVerb(t, _fakeDimensionClassifier)).ToList();
        container.Action = string.Join(";", actions);

        var domains = tags.Where(t => _wordRoleClassifier.IsNoun(t, _fakeDimensionClassifier)).ToList();
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