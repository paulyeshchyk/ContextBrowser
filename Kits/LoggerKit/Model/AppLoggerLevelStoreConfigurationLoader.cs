using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LoggerKit.Model;

// context: log, build
public static class AppLoggerLevelStoreConfigurationLoader
{
    // context: log, build
    public static Dictionary<TAppLevel, TLogLevel> LoadConfigurationFile<TAppLevel, TLogLevel>(string filePath, TLogLevel defaultValue)
        where TAppLevel : notnull
        where TLogLevel : notnull
    {
        if (!File.Exists(filePath))
        {
            var defaultConfig = new Dictionary<TAppLevel, TLogLevel>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }

        string jsonContent = File.ReadAllText(filePath);
        var result = LoadConfigurationJson<TAppLevel, TLogLevel>(jsonContent, defaultValue);
        return result;
    }

    // context: log, build
    public static Dictionary<TAppLevel, TLogLevel> LoadConfigurationJson<TAppLevel, TLogLevel>(string jsonContent, TLogLevel defaultValue)
        where TAppLevel : notnull
        where TLogLevel : notnull
    {
        try
        {
            var config = JsonSerializer.Deserialize<LogConfiguration<TAppLevel, TLogLevel>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return LoadConfiguration(defaultValue, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEP][APP  ]Ошибка десериализации конфигурации: {ex.Message}");
            var defaultConfig = new Dictionary<TAppLevel, TLogLevel>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }
    }

    // context: log, build
    public static Dictionary<TAppLevel, TLogLevel> LoadConfiguration<TAppLevel, TLogLevel>(TLogLevel defaultValue, LogConfiguration<TAppLevel, TLogLevel>? config)
        where TAppLevel : notnull
        where TLogLevel : notnull
    {
        if (config == null || config.LogLevels == null)
        {
            var defaultConfig = new Dictionary<TAppLevel, TLogLevel>();
            SetDefaultLogLevels(defaultConfig, defaultValue);
            return defaultConfig;
        }

        var result = LoadConfiguration(config, defaultValue);
        return result;
    }

    // context: log, build
    public static Dictionary<TAppLevel, TLogLevel> LoadConfiguration<TAppLevel, TLogLevel>(LogConfiguration<TAppLevel, TLogLevel> data, TLogLevel defaultValue)
        where TAppLevel : notnull
        where TLogLevel : notnull
    {
        var finalConfig = new Dictionary<TAppLevel, TLogLevel>();

        // Фильтруем записи с null, если они вдруг возникли
        var validEntries = data.LogLevels.Where(e => e.AppLevel != null && e.LogLevel != null);

        // Используем ToDictionary, чтобы обработать дубликаты
        var groupedEntries = validEntries.GroupBy(e => e.AppLevel!)
                                         .ToDictionary(g => g.Key, g => g.Last().LogLevel!);

        foreach (var entry in groupedEntries)
        {
            finalConfig[entry.Key] = entry.Value;
        }

        SetDefaultLogLevels(finalConfig, defaultValue);
        return finalConfig;
    }

    // context: log, build
    internal static void SetDefaultLogLevels<TAppLevel, TLogLevel>(Dictionary<TAppLevel, TLogLevel> config, TLogLevel defaultValue)
        where TAppLevel : notnull
        where TLogLevel : notnull
    {
        if (!typeof(TAppLevel).IsEnum)
            return;

        foreach (TAppLevel appLevel in Enum.GetValues(typeof(TAppLevel)))
        {
            if (!config.ContainsKey(appLevel))
            {
                config[appLevel] = defaultValue;
            }
        }
    }
}