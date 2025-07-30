using ContextBrowser.extensions;

namespace ContextBrowser.LoggerKit;

public static class IndentedAppLoggerHelpers
{
    public static string SEndTag = "TreeEnd";
    public static string SStartTag = "TreeStart";
}

public class IndentedAppLogger<T> : AppLogger<T>
    where T : notnull
{
    private readonly int _indenterSize;
    private readonly char _indentedBy;
    private readonly Dictionary<LogKey<T>, int> _indentationLevels = new();
    private readonly object _indentationLock = new();

    public IndentedAppLogger(AppLoggerLevelStore<T> store, ILogWriter writer, char indentedBy = '·', int indenterSize = 4) : base(store, writer)
    {
        _indentedBy = indentedBy;
        _indenterSize = indenterSize;
    }

    protected override void TheWriteFunc(T appLevel, LogLevel logLevel, string message)
    {
        // Создаём ключ один раз для текущего вызова
        var logKey = new LogKey<T>(appLevel, logLevel);

        // Обрабатываем маркеры
        if(message.Equals(IndentedAppLoggerHelpers.SStartTag))
        {
            lock(_indentationLock)
            {
                // Увеличиваем отступ
                _indentationLevels.TryGetValue(logKey, out var currentLevel);
                _indentationLevels[logKey] = currentLevel + 1;
            }
            return; // Не выводим маркер
        }

        if(message.Equals(IndentedAppLoggerHelpers.SEndTag))
        {
            lock(_indentationLock)
            {
                // Уменьшаем отступ
                _indentationLevels.TryGetValue(logKey, out var currentLevel);
                if(currentLevel > 0)
                {
                    _indentationLevels[logKey] = currentLevel - 1;
                }
            }
            return; // Не выводим маркер
        }

        // Форматируем сообщение с отступом
        var currentIndentation = 0;
        lock(_indentationLock)
        {
            _indentationLevels.TryGetValue(logKey, out currentIndentation);
        }

        var indentation = new string(_indentedBy, currentIndentation * _indenterSize);
        var formattedMessage = $"{indentation}{message}";

        _writer.Write(FormattedText(appLevel, logLevel, formattedMessage));
    }
}

public record LogKey<TAppLevel>(TAppLevel AppLevel, LogLevel LogLevel)
    where TAppLevel : notnull;