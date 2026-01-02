using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Comment;
using ContextKit.Model;
using LoggerKit;

namespace ContextKit.ContextData;

// context: roslyn, contextInfo, build
public class ContextValidationDecorator<T> : ICommentParsingStrategy<T>
    where T : IContextWithReferences<T>
{
    private readonly ICommentParsingStrategy<T> _strategy;
    private readonly IAppLogger<AppLevel> _logger;

    public ContextValidationDecorator(ICommentParsingStrategy<T> strategy, IAppLogger<AppLevel> logger)
    {
        _strategy = strategy;
        _logger = logger;
    }

    // context: roslyn, contextInfo, build
    public void Execute(T? container, string comment)
    {
        if (container == null)
        {
            _logger.WriteLog(AppLevel.R_Comments, LogLevel.Err, "Comment container is null");
            return;
        }

        _strategy.Execute(container, comment);

        if (string.IsNullOrEmpty(container.Action))
        {
            //container.Action = _contextClassifier.FakeAction;
            _logger.WriteLog(AppLevel.R_Comments, LogLevel.Dbg, $"[{container.Name}]: No action found");
        }

        if (container.Domains.Count == 0)
        {
            //container.Domains.Add(_contextClassifier.FakeDomain);
            _logger.WriteLog(AppLevel.R_Comments, LogLevel.Dbg, $"[{container.Name}]: No domains found");
        }
    }
}