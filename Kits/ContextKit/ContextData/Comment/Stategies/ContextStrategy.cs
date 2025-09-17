using System;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;

namespace ContextKit.Stategies;

public class ContextStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "context";

    private readonly IDomainPerActionContextClassifier _contextClassifier;
    private readonly IAppLogger<AppLevel> _logger;

    public ContextStrategy(IDomainPerActionContextClassifier contextClassifier, IAppLogger<AppLevel> logger)
    {
        _contextClassifier = contextClassifier;
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

        var actions = tags.Where(t => _contextClassifier.IsVerb(t)).ToList();
        container.Action = string.Join(";", actions);

        var domains = tags.Where(t => _contextClassifier.IsNoun(t)).ToList();
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