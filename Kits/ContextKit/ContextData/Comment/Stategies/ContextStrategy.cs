using System;
using System.Linq;
using ContextKit.Model;

namespace ContextKit.Stategies;

public class ContextStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "context";

    private readonly IContextClassifier _contextClassifier;

    public ContextStrategy(IContextClassifier contextClassifier)
    {
        _contextClassifier = contextClassifier;
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
            //_onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Comment container is null");
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