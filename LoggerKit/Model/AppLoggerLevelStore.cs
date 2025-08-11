using ContextBrowserKit.Log.Options;

namespace LoggerKit.Model;

// context: log, share
public class AppLoggerLevelStore<T>
    where T : notnull
{
    private readonly Dictionary<T, LogLevel> _levels = new();

    // Объект для синхронизации доступа к словарю из разных потоков.
    private readonly object _lock = new();

    // context: log, update
    public void SetLevel(T sectionId, LogLevel level)
    {
        lock(_lock)
        {
            _levels[sectionId] = level;
        }
    }

    public void SetLevels(List<LogConfigEntry<T, LogLevel>> newLevels)
    {
        lock(_lock)
        {
            _levels.Clear();

            foreach(var entry in newLevels)
            {
                if(entry.AppLevel is T appLevel)
                {
                    _levels[appLevel] = entry.LogLevel;
                }
            }
        }
    }

    // context: log, update
    public void SetLevels(Dictionary<T, LogLevel> newLevels)
    {
        if(newLevels == null)
        {
            return;
        }

        lock(_lock)
        {
            _levels.Clear();

            foreach(var entry in newLevels)
            {
                _levels[entry.Key] = entry.Value;
            }
        }
    }

    // context: log, read
    public LogLevel GetLevel(T sectionId)
    {
        lock(_lock)
        {
            return _levels.TryGetValue(sectionId, out var level) ? level : LogLevel.None;
        }
    }

    // context: log, read
    public void LoadFromDictionary(Dictionary<T, LogLevel> newLevels)
    {
        if(newLevels == null)
        {
            return;
        }

        lock(_lock)
        {
            _levels.Clear();

            foreach(var entry in newLevels)
            {
                _levels[entry.Key] = entry.Value;
            }
        }
    }
}