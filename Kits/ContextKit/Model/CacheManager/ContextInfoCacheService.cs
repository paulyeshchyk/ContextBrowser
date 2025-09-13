using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Factory;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public interface IContextInfoCacheService
{
    // context: relations, build
    Task<IEnumerable<ContextInfo>> GetOrParseAndCacheAsync(
        CacheJsonModel cacheModel,
        Func<CancellationToken, Task<IEnumerable<ContextInfo>>> parseJob,
        Func<List<ContextInfoSerializableModel>, CancellationToken, Task<List<ContextInfo>>> onRelationCallback,
        CancellationToken cancellationToken);
}

/// <summary>
/// Управляет сохранением и чтением списка объектов ContextInfo,
/// используя промежуточную модель для обхода проблем сериализации.
/// </summary>
// context: relations, build
public class ContextInfoCacheService : IContextInfoCacheService
{
    private readonly IAppLogger<AppLevel> _appLogger;

    // Кэш в памяти, который будет хранить "обещание" (Task) результата парсинга.
    private Task<IEnumerable<ContextInfo>>? _inMemoryCacheTask;

    // Семафор для синхронизации доступа к _inMemoryCacheTask.
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private readonly IContextInfoRelationManager _relationManager;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public ContextInfoCacheService(IAppLogger<AppLevel> appLogger, IContextInfoRelationManager relationManager)
    {
        _appLogger = appLogger;
        _relationManager = relationManager;
    }

    /// <summary>
    /// Единый метод для получения распарсенных данных.
    /// Он сначала проверяет кэш в памяти, потом на диске,
    /// и только если кэша нет, запускает парсинг.
    /// </summary>
    public async Task<IEnumerable<ContextInfo>> GetOrParseAndCacheAsync(
        CacheJsonModel cacheModel,
        Func<CancellationToken, Task<IEnumerable<ContextInfo>>> parseJob,
        Func<List<ContextInfoSerializableModel>, CancellationToken, Task<List<ContextInfo>>> onRelationCallback,
        CancellationToken cancellationToken)
    {
        // Сначала пытаемся быстро получить результат из кэша в памяти
        if (_inMemoryCacheTask != null && _inMemoryCacheTask.IsCompletedSuccessfully)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, "Returning data from in-memory cache.");
            return _inMemoryCacheTask.Result;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Проверяем ещё раз, так как между проверкой и WaitAsync мог завершиться другой поток.
            if (_inMemoryCacheTask != null && _inMemoryCacheTask.IsCompletedSuccessfully)
            {
                return _inMemoryCacheTask.Result;
            }

            // Пытаемся прочитать из файла
            var contextsFromFile = await ReadContextsFromCache(
                cacheModel,
                (token) => Task.FromResult<IEnumerable<ContextInfo>>(Enumerable.Empty<ContextInfo>()),
                onRelationCallback,
                cancellationToken);

            if (contextsFromFile.Any())
            {
                _inMemoryCacheTask = Task.FromResult(contextsFromFile);
                _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, "Returning data from file cache.");
                return contextsFromFile;
            }

            // Если кэша нет, запускаем парсинг
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, "Cache not found. Starting parsing job.");

            // Запускаем парсинг и сохраняем "обещание" результата в поле класса
            _inMemoryCacheTask = parseJob(cancellationToken);

            // Не ждём, так как результат парсинга уже есть в _inMemoryCacheTask
            var parsingResult = await _inMemoryCacheTask;

            SaveOnBackground(cacheModel, parsingResult, cancellationToken);

            return parsingResult;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void SaveOnBackground(CacheJsonModel cacheModel, IEnumerable<ContextInfo> parsingResult, CancellationToken cancellationToken)
    {
        // Запускаем "fire-and-forget" задачу для сохранения на диск
        _ = Task.Run(async () =>
        {
            await FileCacheService.SaveToFile(cacheModel, parsingResult, cancellationToken).ConfigureAwait(false);
        }, cancellationToken);
    }

    public async Task<IEnumerable<ContextInfo>> ReadContextsFromCache(CacheJsonModel cacheModel, Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, Func<List<ContextInfoSerializableModel>, CancellationToken, Task<List<ContextInfo>>> onRelationCallback, CancellationToken cancellationToken)
    {
        if (FileCacheService.ShouldRebuildCache(cacheModel))
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Renewing cache");
            return await fallback(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            string fileContent = FileCacheService.ReadFromFile(cacheModel);
            if (string.IsNullOrEmpty(fileContent))
            {
                return await fallback(cancellationToken).ConfigureAwait(false);
            }

            return await DeserializeContent(fallback, onRelationCallback, fileContent, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, ex.Message);
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IEnumerable<ContextInfo>> DeserializeContent(Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, Func<List<ContextInfoSerializableModel>, CancellationToken, Task<List<ContextInfo>>> onRelationCallback, string fileContent, CancellationToken cancellationToken)
    {
        try
        {
            var serializableList = FileCacheService.DeserializeFileContent(fileContent);
            if (serializableList != null)
            {
                return await onRelationCallback(serializableList, cancellationToken).ConfigureAwait(false);
            }
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, ex.Message);
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
    }
}

internal class FileCacheService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static bool ShouldRebuildCache(CacheJsonModel cacheModel)
    {
        return !File.Exists(cacheModel.Input) || cacheModel.RenewCache;
    }

    public static List<ContextInfoSerializableModel>? DeserializeFileContent(string fileContent)
    {
        return JsonSerializer.Deserialize<List<ContextInfoSerializableModel>>(fileContent, _jsonOptions);
    }

    public static string ReadFromFile(CacheJsonModel cacheModel)
    {
        return File.ReadAllText(cacheModel.Input);
    }

    public static async Task SaveToFile(CacheJsonModel cacheModel, IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        if (File.Exists(cacheModel.Output))
        {
            try
            {
                File.Delete(cacheModel.Output);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cache can't be erased\n{ex}");
            }
        }

        try
        {
            var serializableList = ContextInfoSerializableModelAdapter.Adapt(contextsList.ToList());

            var json = JsonSerializer.Serialize(serializableList, _jsonOptions);
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Contexts list has no items");
            }

            var directoryPath = Path.GetDirectoryName(cacheModel.Output);
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new Exception($"directoryPath is empty for cache output file ({cacheModel.Output})");
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllTextAsync(cacheModel.Output, json, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception($"Cache can't be saved\n{ex}");
        }
    }
}
