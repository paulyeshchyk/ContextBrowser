using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;

namespace LoggerKit.Model;

public static class IndentedAppLoggerHelpers1
{
    public static string SEndTag = "TreeEnd";
    public static string SStartTag = "TreeStart";
}

// context: log, share
public class IndentedAppLogger<T> : AppLogger<T>
    where T : notnull
{
    private readonly int _indenterSize;
    private readonly char _indentedBy;
    private readonly Dictionary<object, int> _indentationLevels = new();
    private readonly object _indentationLock = new();
    private readonly bool _printEmptyNodes;

    // Добавляем словарь для хранения зависимостей Child -> Parent
    private readonly Dictionary<T, T> _dependencies;

    public IndentedAppLogger(AppLoggerLevelStore<T> store, ILogWriter writer, char indentedBy = '·', int indenterSize = 4, bool printEmptyNodes = false) : base(store, writer)
    {
        _indentedBy = indentedBy;
        _indenterSize = indenterSize;
        _printEmptyNodes = printEmptyNodes;
        _dependencies = new Dictionary<T, T>();
    }

    // Дополнительный конструктор для передачи словаря зависимостей
    public IndentedAppLogger(AppLoggerLevelStore<T> store, ILogWriter writer, Dictionary<T, T> dependencies, char indentedBy = '·', int indenterSize = 4, bool printEmptyNodes = false)
        : base(store, writer)
    {
        _indentedBy = indentedBy;
        _indenterSize = indenterSize;
        _printEmptyNodes = printEmptyNodes;
        _dependencies = dependencies ?? new Dictionary<T, T>();
    }

    // context: log, share
    protected override void TheWriteFunc(T appLevel, LogObject logObject)
    {
        // Создаём ключ один раз для текущего вызова
        var logKey = BuildIndentionKey(appLevel, logObject.LogLevel);

        switch (logObject.LogLevelNode)
        {
            case LogLevelNode.Start:
                IndentLogNodeStart(appLevel, logObject, logKey);
                break;

            case LogLevelNode.End:
                IndentLogNodeEnd(appLevel, logObject, logKey);
                break;

            default:
                IndentLogNodeNone(appLevel, logObject, logKey);
                break;
        }
    }

    private void IndentLogNodeNone(T appLevel, LogObject logObject, object logKey)
    {
        var formattedMessage = FormatIndentedLines(logObject.Message, logKey);
        foreach (var l in formattedMessage)
            _writer.Write(FormattedText(appLevel, logObject.LogLevel, l));
    }

    private void IndentLogNodeEnd(T appLevel, LogObject logObject, object logKey)
    {
        lock (_indentationLock)
        {
            // Уменьшаем отступ
            _indentationLevels.TryGetValue(logKey, out var currentLevel);
            if (currentLevel > 0)
            {
                _indentationLevels[logKey] = currentLevel - 1;
            }

            var message = logObject.Message;

            // Пишем после изменения отступа
            bool canPrintEndNode = (!string.IsNullOrWhiteSpace(message)) || _printEmptyNodes;
            if (canPrintEndNode)
            {
                var endformattedMessage = FormatIndentedLines(message, logKey);
                foreach (var l in endformattedMessage)
                {
                    _writer.Write(FormattedText(appLevel, logObject.LogLevel, l));
                }
            }
        }
    }

    private void IndentLogNodeStart(T appLevel, LogObject logObject, object logKey)
    {
        lock (_indentationLock)
        {
            var message = logObject.Message;

            // Пишем до изменения отступа
            bool canPrintStartNode = (!string.IsNullOrWhiteSpace(message)) || _printEmptyNodes;
            if (canPrintStartNode)
            {
                var startformattedMessage = FormatIndentedLines(message, logKey);
                foreach (var l in startformattedMessage)
                    _writer.Write(FormattedText(appLevel, logObject.LogLevel, l));
            }

            // Увеличиваем отступ
            _indentationLevels.TryGetValue(logKey, out var currentLevel);
            _indentationLevels[logKey] = currentLevel + 1;
        }
    }

    // context: log, share
    protected string[] FormatIndentedLines(string? message, object logKey)
    {
        var theMessage = string.IsNullOrWhiteSpace(message) ? string.Empty : message;

        var currentIndentation = 0;
        lock (_indentationLock)
        {
            if (_indentationLevels.TryGetValue(logKey, out var currentLevel))
            {
                currentIndentation = currentLevel;
            }

            if (_dependencies.Any())
            {
                if (logKey is T appLevel)
                {
                    var parentAppLevel = appLevel;
                    while (_dependencies.TryGetValue(parentAppLevel, out var nextParent))
                    {
                        if (_indentationLevels.TryGetValue(nextParent, out var parentLevel))
                        {
                            currentIndentation += parentLevel;
                        }
                        parentAppLevel = nextParent;
                    }
                }
            }
        }

        var indentation = new string(_indentedBy, currentIndentation * _indenterSize);

        // 1. Создаем массив строк-разделителей.
        string[] separators = { "\r\n", "\n\r", "\n", "\r" };

        // 2. Разбиваем сообщение. StringSplitOptions.None сохраняет пустые строки.
        var lines = theMessage.Split(separators, StringSplitOptions.None);

        // 3. Добавляем отступ к каждой строке.
        var indentedLines = lines.Select(line => indentation + line).ToArray();

        return indentedLines;
    }

    // context: log, share
    protected static object BuildIndentionKey(T appLevel, LogLevel logLevel)
    {
        return appLevel;
    }
}