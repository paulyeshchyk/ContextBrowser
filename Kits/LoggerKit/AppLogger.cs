using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using LoggerKit.Extensions;
using LoggerKit.Model;

namespace LoggerKit;

// context: log, share
public interface IAppLogger<T>
    where T : notnull
{
    void Configure(LogConfiguration<T, LogLevel> configuration);

    // context: log, share
    void WriteLog(T appLevel, LogLevel logLevel, string message, LogLevelNode logLevelNode = LogLevelNode.None);

    // context: log, share
    void WriteLogObject(T appLevel, LogObject logObject);
}

// context: log, share
public class AppLogger<T> : IAppLogger<T> where T : notnull
{
    private readonly AppLoggerLevelStore<T> _appLogLevelStorage;
    protected readonly ILogWriter _writer;

    public AppLogger(AppLoggerLevelStore<T> store, ILogWriter writer)
    {
        _appLogLevelStorage = store;
        _writer = writer;
    }

    public void Configure(LogConfiguration<T, LogLevel> configuration)
    {
        _appLogLevelStorage.SetLevels(configuration.LogLevels);
    }

    // context: log, build
    protected string FormattedText(T appLevel, LogLevel logLevel, string message)
    {
        var applvl = LogerExtensions.BuildFormatterPrefix(appLevel);
        var loglvl = LogerExtensions.BuildFormatterPrefix(logLevel);
        return $"[{loglvl}][{applvl}]:{message}";
    }

    // context: log, model
    private bool CanWriteLog(LogLevel requested, LogLevel limitedByAppLevel)
    {
        if (requested == LogLevel.None)
        {
            return false;
        }

        return requested <= limitedByAppLevel;
    }

    // context: log, share
    // parsing: error
    public void WriteLog(T appLevel, LogLevel logLevel, string message, LogLevelNode logLevelNode = LogLevelNode.None)
    {
        var logObject = new LogObject(logLevel, message, logLevelNode);
        WriteLogObject(appLevel, logObject);
    }

    // context: log, share
    // parsing: error
    public void WriteLogObject(T appLevel, LogObject logObject)
    {
        var limitedByAppLevel = _appLogLevelStorage.GetLevel(appLevel);

        if (CanWriteLog(logObject.LogLevel, limitedByAppLevel))
        {
            TheWriteFunc(appLevel, logObject);
        }
    }

    // context: log, share
    protected virtual void TheWriteFunc(T appLevel, LogObject logObject)
    {
        var formattedMessage = FormattedText(appLevel, logObject.LogLevel, logObject.Message ?? string.Empty);
        _writer.Write(formattedMessage);
    }
}
