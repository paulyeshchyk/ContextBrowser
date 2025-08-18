using ContextBrowserKit.Log.Options;

namespace ContextBrowserKit.Log;

public class LogObject
{
    public LogObject(LogLevel logLevel, string? message = default, LogLevelNode logLevelNode = LogLevelNode.None)
    {
        LogLevel = logLevel;
        _message = message;
        LogLevelNode = logLevelNode;
    }

    public LogLevel LogLevel { get; set; }

    private string? _message;

    public string? Message => MessageWithCode();

    public LogLevelNode LogLevelNode { get; set; }

    private string? MessageWithCode()
    {
        return _message;
    }
}
