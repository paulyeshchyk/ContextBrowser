using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace ContextKit.Stategies;

// context: roslyn, contextInfo, build
public class ContextValidationDecorator<T> : ICommentParsingStrategy<T>
    where T : ContextInfo
{
    private readonly ICommentParsingStrategy<T> _strategy;
    private readonly OnWriteLog? _onWriteLog;
    private readonly IContextClassifier _contextClassifier;

    public ContextValidationDecorator(IContextClassifier contextClassifier, ICommentParsingStrategy<T> strategy, OnWriteLog? onWriteLog = null)
    {
        _strategy = strategy;
        _onWriteLog = onWriteLog;
        _contextClassifier = contextClassifier;
    }

    // context: roslyn, contextInfo, build
    public void Execute(T? container, string comment)
    {
        if (!(container != null && container is not null))
        {
            _onWriteLog?.Invoke(AppLevel.R_Comments, LogLevel.Err, "Comment container is null");
            return;
        }

        _strategy.Execute(container, comment);

        if (string.IsNullOrEmpty(container.Action))
        {
            container.Action = _contextClassifier.FakeAction;
            _onWriteLog?.Invoke(AppLevel.R_Comments, LogLevel.Dbg, $"[{container.Name}]: No action found");
        }

        if (container.Domains.Count == 0)
        {
            container.Domains.Add(_contextClassifier.FakeDomain);
            _onWriteLog?.Invoke(AppLevel.R_Comments, LogLevel.Dbg, $"[{container.Name}]: No domains found");
        }
    }
}