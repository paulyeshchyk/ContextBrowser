using ContextBrowser.extensions;

namespace ContextBrowser.LoggerKit;

public class AppLogger<T>
    where T : notnull
{
    private readonly AppLoggerLevelStore<T> _appLogLevelStorage;
    private readonly LogWriter _writer;

    public AppLogger(AppLoggerLevelStore<T> store, LogWriter writer)
    {
        _appLogLevelStorage = store;
        _writer = writer;
    }

    private bool CanWriteLog(LogLevel requested, LogLevel limitedByAppLevel)
    {
        if(requested == LogLevel.None)
        {
            return false;
        }

        return (int)requested <= (int)limitedByAppLevel;
    }

    public void WriteLog(T appLevel, LogLevel logLevel, string message)
    {
        var level = _appLogLevelStorage.GetLevel(appLevel);

        if(CanWriteLog(logLevel, level))
        {
            _writer(message);
        }
    }
}


public delegate void LogWriter(string message);

public delegate void OnWriteLog(AppLevel al, LogLevel ll, string message);
