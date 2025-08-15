using ContextBrowserKit.Log.Options;
using LoggerKit.Model;

namespace LoggerKit;

// context: log, build
public class AppLogger<T>
    where T : notnull
{
    private readonly AppLoggerLevelStore<T> _appLogLevelStorage;
    protected readonly ILogWriter _writer;

    public AppLogger(AppLoggerLevelStore<T> store, ILogWriter writer)
    {
        _appLogLevelStorage = store;
        _writer = writer;
    }

    // context: log, build
    protected string FormattedPrefix(object dataToFormat)
    {
        // Получаем строковое представление AppLevel
        string prefix = dataToFormat.ToString() ?? string.Empty;

        // Задаём желаемую длину префикса
        const int prefixLength = 5;

        // Обрезаем или дополняем префикс
        if(prefix.Length > prefixLength)
        {
            // Обрезаем, если префикс длиннее
            prefix = prefix.Substring(0, prefixLength);
        }
        else if(prefix.Length < prefixLength)
        {
            // Дополняем пробелами, если префикс короче
            // ' ' - символ-заполнитель, prefixLength - целевая длина
            prefix = prefix.PadRight(prefixLength, ' ');
        }

        // Возвращаем отформатированную строку
        return prefix.ToUpper();
    }

    // context: log, build
    protected string FormattedText(T appLevel, LogLevel logLevel, string message)
    {
        var applvl = FormattedPrefix(appLevel);
        var loglvl = FormattedPrefix(logLevel);
        return $"[{loglvl}][{applvl}]:{message}";
    }

    // context: log, model
    private bool CanWriteLog(LogLevel requested, LogLevel limitedByAppLevel)
    {
        if(requested == LogLevel.None)
        {
            return false;
        }

        return (int)requested <= (int)limitedByAppLevel;
    }

    // context: log, build
    // parsing: error
    public void WriteLog(T appLevel, LogLevel logLevel, string message, LogLevelNode logLevelNode = LogLevelNode.None)
    {
        var limitedByAppLevel = _appLogLevelStorage.GetLevel(appLevel);

        if(CanWriteLog(logLevel, limitedByAppLevel))
        {
            TheWriteFunc(appLevel, logLevel, message, logLevelNode);
        }
    }

    // context: log, build
    protected virtual void TheWriteFunc(T appLevel, LogLevel logLevel, string message, LogLevelNode logLevelNode = LogLevelNode.None)
    {
        var formattedMessage = FormattedText(appLevel, logLevel, message);
        _writer.Write(formattedMessage);
    }
}
