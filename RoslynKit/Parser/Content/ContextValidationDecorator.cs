using ContextBrowser.ContextKit.Parser.CommentParser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;

namespace RoslynKit.Parser.Content;

// context: csharp, contextInfo, build
public class ContextValidationDecorator<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    private readonly ICommentParsingStrategy<T> _strategy;
    private readonly OnWriteLog? _onWriteLog;

    public ContextValidationDecorator(ICommentParsingStrategy<T> strategy, OnWriteLog? onWriteLog = null)
    {
        _strategy = strategy;
        _onWriteLog = onWriteLog;
    }

    // context: csharp, contextInfo, build
    public void Execute(string comment, T container)
    {
        _strategy.Execute(comment, container);

        if(string.IsNullOrEmpty(container.Action))
        {
            container.Action = ContextClassifier.FakeAction;
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{container.Name}]: No action found");
        }

        if(container.Domains.Count == 0)
        {
            container.Domains.Add(ContextClassifier.FakeDomain);
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Warn, $"[{container.Name}]: No domains found");
        }
    }
}