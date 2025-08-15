using ContextBrowserKit.Log.Options;

namespace LoggerKit.Model;

// context: log, share
public class AppLoggerLevelStore<TAppLevel>
    where TAppLevel : notnull
{
    private readonly Dictionary<TAppLevel, LogLevel> _levels = new();

    // Объект для синхронизации доступа к словарю из разных потоков.
    private readonly object _lock = new();

    // context: log, update
    public void SetLevel(TAppLevel sectionId, LogLevel level)
    {
        lock (_lock)
        {
            _levels[sectionId] = level;
        }
    }

    public void SetLevels(List<LogConfigEntry<TAppLevel, LogLevel>> newLevels)
    {
        lock (_lock)
        {
            _levels.Clear();

            foreach (var entry in newLevels)
            {
                if (entry.AppLevel is TAppLevel appLevel)
                {
                    _levels[appLevel] = entry.LogLevel;
                }
            }
        }
    }

    // context: log, update
    public void SetLevels(Dictionary<TAppLevel, LogLevel> newLevels)
    {
        if (newLevels == null)
        {
            return;
        }

        lock (_lock)
        {
            _levels.Clear();

            foreach (var entry in newLevels)
            {
                _levels[entry.Key] = entry.Value;
            }
        }
    }

    // context: log, read
    public LogLevel GetLevel(TAppLevel sectionId)
    {
        lock (_lock)
        {
            return _levels.TryGetValue(sectionId, out var level) ? level : LogLevel.None;
        }
    }

    // context: log, read
    public void LoadFromDictionary(Dictionary<TAppLevel, LogLevel> newLevels)
    {
        if (newLevels == null)
        {
            return;
        }

        lock (_lock)
        {
            _levels.Clear();

            foreach (var entry in newLevels)
            {
                _levels[entry.Key] = entry.Value;
            }
        }
    }
}