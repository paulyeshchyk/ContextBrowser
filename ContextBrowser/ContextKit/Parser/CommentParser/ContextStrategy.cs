using ContextBrowser.ContextKit.Model;
using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.ContextKit.Parser.CommentParser;

public class ContextStrategy<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    public static string Keyword => "context";

    private readonly IContextClassifier _contextClassifier;
    private OnWriteLog? _onWriteLog;

    public ContextStrategy(IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        _contextClassifier = contextClassifier;
        _onWriteLog = onWriteLog;
    }

    public void Execute(string comment, T container)
    {
        var content = CommentWithKeywordParser.ExtractContent(Keyword, comment);

        if(string.IsNullOrEmpty(content))
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "No action/domain found");
            return;
        }

        var tags = content.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim().ToLowerInvariant());

        var actions = tags.Where(t => _contextClassifier.IsVerb(t)).ToList();
        container.Action = string.Join(";", actions);

        var domains = tags.Where(t => _contextClassifier.IsNoun(t)).ToList();
        foreach(var domain in domains)
        {
            container.Domains.Add(domain.ToLower());
        }

        foreach(var tag in tags)
        {
            container.Contexts.Add(tag);
        }

        if(!actions.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "No action found");
        }
        if(!domains.Any())
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, "No domain found");
        }
    }
}