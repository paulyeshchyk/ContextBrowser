using System.Text.Json;

namespace LoggerKit.Model;

// context: log, build
public static class AppLoggerLevelStoreConfigurationLoader
{
    // context: log, build
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
        var result = LoadConfigurationJson<A, L>(jsonContent, defaultValue);
        return result;
    }

    // context: log, build
    public static Dictionary<A, L> LoadConfigurationJson<A, L>(string jsonContent, L defaultValue)
        where A : notnull
        where L : notnull
    {
        try
        {
            var config = JsonSerializer.Deserialize<LogConfiguration<A, L>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return LoadConfiguration(defaultValue, config);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка десериализации конфигурации: {ex.Message}");
            var defaultConfig = new Dictionary<A, L>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }
    }

    // context: log, build
    public static Dictionary<A, L> LoadConfiguration<A, L>(L defaultValue, LogConfiguration<A, L>? config)
        where A : notnull
        where L : notnull
    {
        if(config == null || config.LogLevels == null)
        {
            var defaultConfig = new Dictionary<A, L>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }

        var result = LoadConfiguration(config, defaultValue);
        return result;
    }

    // context: log, build
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

    // context: log, build
    internal static void SetDefaultLogLevels<A, L>(Dictionary<A, L> config, L defaultValue)
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