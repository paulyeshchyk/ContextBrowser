using ContextBrowser.extensions;

namespace ContextBrowser.LoggerKit;

public class AppLogger<T>
    where T : notnull
{
    private readonly AppLoggerLevelStore<T> _appLogLevelStorage;
    protected readonly ILogWriter _writer; // Теперь это ILogWriter

    public AppLogger(AppLoggerLevelStore<T> store, ILogWriter writer) // ILogWriter в конструкторе
    {
        _appLogLevelStorage = store;
        _writer = writer;
    }

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

    protected string FormattedText(T appLevel, LogLevel logLevel, string message)
    {
        var applvl = FormattedPrefix(appLevel);
        var loglvl = FormattedPrefix(logLevel);
        return $"[{loglvl}][{applvl}]:{message}";
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
        var limitedByAppLevel = _appLogLevelStorage.GetLevel(appLevel);

        if(CanWriteLog(logLevel, limitedByAppLevel))
        {
            TheWriteFunc(appLevel, logLevel, message);
        }
    }

    protected virtual void TheWriteFunc(T appLevel, LogLevel logLevel, string message)
    {
        var formattedMessage = FormattedText(appLevel, logLevel, message);
        _writer.Write(formattedMessage);
    }
}


public delegate void LogWriter(string message);

public delegate void OnWriteLog(AppLevel al, LogLevel ll, string message);

public interface ILogWriter
{
    void Write(string message);
}

public class ConsoleLogWriter : ILogWriter
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}