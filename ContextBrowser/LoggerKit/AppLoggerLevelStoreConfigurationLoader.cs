using System.Text.Json;

namespace ContextBrowser.LoggerKit;

public class LogConfigEntry<A, L>
    where A : notnull
    where L : notnull
{
    public required A AppLevel { get; set; }

    public required L LogLevel { get; set; }
}

public class LogConfiguration<A, L>
    where A : notnull
    where L : notnull
{
    public List<LogConfigEntry<A, L>> LogLevels { get; set; } = new List<LogConfigEntry<A, L>>();
}

public static class AppLoggerLevelStoreConfigurationLoader
{
    public static Dictionary<A, L> LoadConfigurationFile<A, L>(string filePath, L defaultValue)
        where A : notnull
        where L : notnull
    {
        if(!File.Exists(filePath))
        {
            var defaultConfig = new Dictionary<A, L>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }

        string jsonContent = File.ReadAllText(filePath);
        return LoadConfigurationJson<A, L>(jsonContent, defaultValue);
    }

    public static Dictionary<A, L> LoadConfigurationJson<A, L>(string jsonContent, L defaultValue)
        where A : notnull
        where L : notnull
    {
        try
        {
            var config = JsonSerializer.Deserialize<LogConfiguration<A, L>>(jsonContent, new JsonSerializerOptions
            {
                // Это опция для обработки регистронезависимости,
                // но лучше просто исправить имена в JSON.
                PropertyNameCaseInsensitive = true
            });

            if(config == null || config.LogLevels == null)
            {
                var defaultConfig = new Dictionary<A, L>();
                SetDefaultLogLevels(defaultConfig, defaultValue);
                return defaultConfig;
            }

            return LoadConfiguration(config, defaultValue);
        }
        catch(Exception ex)
        {
            // Здесь обрабатываем ошибки десериализации, например, неверные имена enum
            Console.WriteLine($"Ошибка десериализации конфигурации: {ex.Message}");
            var defaultConfig = new Dictionary<A, L>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }
    }

    public static Dictionary<A, L> LoadConfiguration<A, L>(LogConfiguration<A, L> data, L defaultValue)
        where A : notnull
        where L : notnull
    {
        var finalConfig = new Dictionary<A, L>();

        // Фильтруем записи с null, если они вдруг возникли
        var validEntries = data.LogLevels.Where(e => e.AppLevel != null && e.LogLevel != null);

        // Используем ToDictionary, чтобы обработать дубликаты
        var groupedEntries = validEntries.GroupBy(e => e.AppLevel!)
                                         .ToDictionary(g => g.Key, g => g.Last().LogLevel!);

        foreach(var entry in groupedEntries)
        {
            finalConfig[entry.Key] = entry.Value;
        }

        SetDefaultLogLevels(finalConfig, defaultValue);
        return finalConfig;
    }

    private static void SetDefaultLogLevels<A, L>(Dictionary<A, L> config, L defaultValue)
        where A : notnull
        where L : notnull
    {
        if(!typeof(A).IsEnum)
            return; // Проверка, что A является enum

        foreach(A appLevel in Enum.GetValues(typeof(A)))
        {
            if(!config.ContainsKey(appLevel))
            {
                config[appLevel] = defaultValue;
            }
        }
    }
}