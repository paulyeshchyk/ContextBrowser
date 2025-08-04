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

    // context: log, share
    protected string FormatIndentedMessage(string message, object logKey)
    {
        // Форматируем сообщение с отступом
        var currentIndentation = 0;
        lock(_indentationLock)
        {
            // Проверяем, существует ли ключ в словаре уровней.
            // Если да, получаем его значение.
            if(_indentationLevels.TryGetValue(logKey, out var currentLevel))
            {
                currentIndentation = currentLevel;
            }

            // Если словарь зависимостей не пуст, добавляем отступы родительских уровней
            if(_dependencies.Any())
            {
                if(logKey is T appLevel)
                {
                    // Рекурсивно поднимаемся по цепочке зависимостей
                    var parentAppLevel = appLevel;
                    while(_dependencies.TryGetValue(parentAppLevel, out var nextParent))
                    {
                        if(_indentationLevels.TryGetValue(nextParent, out var parentLevel))
                        {
                            currentIndentation += parentLevel;
                        }
                        parentAppLevel = nextParent;
                    }
                }
            }
        }

        var indentation = new string(_indentedBy, currentIndentation * _indenterSize);
        var formattedMessage = $"{indentation}{message}";
        return formattedMessage;
    }

    // context: log, share
    protected static object BuildIndentionKey(T appLevel, LogLevel logLevel)
    {
        return appLevel;
    }
}