using ContextBrowser.extensions;

namespace ContextBrowser.LoggerKit;

public static class IndentedAppLoggerHelpers1
{
    public static string SEndTag = "TreeEnd";
    public static string SStartTag = "TreeStart";
}

public class IndentedAppLogger<T> : AppLogger<T>
    where T : notnull
{
    private readonly int _indenterSize;
    private readonly char _indentedBy;
    private readonly Dictionary<object, int> _indentationLevels = new();
    private readonly object _indentationLock = new();
    private readonly bool _printEmptyNodes;

    public IndentedAppLogger(AppLoggerLevelStore<T> store, ILogWriter writer, char indentedBy = '·', int indenterSize = 4, bool printEmptyNodes = false) : base(store, writer)
    {
        _indentedBy = indentedBy;
        _indenterSize = indenterSize;
        _printEmptyNodes = printEmptyNodes;
    }

    protected override void TheWriteFunc(T appLevel, LogLevel logLevel, string message, LogLevelNode logLevelNode = LogLevelNode.None)
    {
        // Создаём ключ один раз для текущего вызова
        var logKey = BuildIndentionKey(appLevel, logLevel);


        switch(logLevelNode)
        {
            case LogLevelNode.Start:
                lock(_indentationLock)
                {
                    // Пишем до изменения отступа
                    bool canPrintStartNode = (!string.IsNullOrWhiteSpace(message)) || _printEmptyNodes;
                    if(canPrintStartNode)
                    {
                        var startformattedMessage = FormatIndentedMessage(message, logKey);
                        _writer.Write(FormattedText(appLevel, logLevel, startformattedMessage));
                    }

                    // Увеличиваем отступ
                    _indentationLevels.TryGetValue(logKey, out var currentLevel);
                    _indentationLevels[logKey] = currentLevel + 1;
                }
                break;
            case LogLevelNode.End:
                lock(_indentationLock)
                {
                    // Уменьшаем отступ
                    _indentationLevels.TryGetValue(logKey, out var currentLevel);
                    if(currentLevel > 0)
                    {
                        _indentationLevels[logKey] = currentLevel - 1;
                    }

                    // Пишем после изменения отступа
                    bool canPrintEndNode = (!string.IsNullOrWhiteSpace(message)) || _printEmptyNodes;
                    if(canPrintEndNode)
                    {
                        var endformattedMessage = FormatIndentedMessage(message, logKey);
                        _writer.Write(FormattedText(appLevel, logLevel, endformattedMessage));
                    }
                }
                break;
            default:
                var formattedMessage = FormatIndentedMessage(message, logKey);
                _writer.Write(FormattedText(appLevel, logLevel, formattedMessage));
                break;
        }
    }

    private string FormatIndentedMessage(string message, object logKey)
    {
        // Форматируем сообщение с отступом
        var currentIndentation = 0;
        lock(_indentationLock)
        {
            _indentationLevels.TryGetValue(logKey, out currentIndentation);
        }

        var indentation = new string(_indentedBy, currentIndentation * _indenterSize);
        var formattedMessage = $"{indentation}{message}";
        return formattedMessage;
    }

    private static object BuildIndentionKey(T appLevel, LogLevel logLevel)
    {
        return appLevel;
        //return new IndentionExtKey<T, LogLevel>(appLevel, logLevel);
    }
}

internal record IndentionExtKey<A, L>(A AppLevel, L LogLevel)
    where A : notnull
    where L : notnull;