using System.Text.Json;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public interface IContextInfoCacheService
{
    // context: relations, build
    Task<IEnumerable<ContextInfo>> ReadContextsFromCache(CacheJsonModel cacheModel, Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, CancellationToken cancellationToken);
    // context: relations, update
    Task SaveContextsToCacheAsync(CacheJsonModel cacheModel, IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken);
}

/// <summary>
/// Управляет сохранением и чтением списка объектов ContextInfo,
/// используя промежуточную модель для обхода проблем сериализации.
/// </summary>
// context: relations, build
public class ContextInfoCacheService : IContextInfoCacheService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IContextInfoRelationManager _relationManager;

    public ContextInfoCacheService(IContextInfoRelationManager relationManager, IAppLogger<AppLevel> appLogger)
    {
        _appLogger = appLogger;
        _relationManager = relationManager;
    }

    /// <summary>
    /// Асинхронно сохраняет список контекстов в файл.
    /// </summary>
    // context: relations, update
    public async Task SaveContextsToCacheAsync(CacheJsonModel cacheModel, IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
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

    /// <summary>
    /// Читает список контекстов из файла и восстанавливает связи.
    /// </summary>
    // context: relations, build
    public async Task<IEnumerable<ContextInfo>> ReadContextsFromCache(CacheJsonModel cacheModel, Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, CancellationToken cancellationToken)
    {
        if (!File.Exists(cacheModel.Input) || cacheModel.RenewCache)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Renewing cache");
            return await fallback(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var fileContent = File.ReadAllText(cacheModel.Input);
            if (string.IsNullOrEmpty(fileContent))
            {
                return await fallback(cancellationToken).ConfigureAwait(false);
            }

            return await DeserializeOrRenew(fallback, fileContent, cancellationToken).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, ex.Message);
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
    }

    // context: relations, build
    internal async Task<IEnumerable<ContextInfo>> DeserializeOrRenew(Func<CancellationToken, Task<IEnumerable<ContextInfo>>> fallback, string fileContent, CancellationToken cancellationToken)
    {
        try
        {
            var serializableList = JsonSerializer.Deserialize<List<ContextInfoSerializableModel>>(fileContent, _jsonOptions);
            if (serializableList == null)
            {
                return await fallback(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return _relationManager.ConvertToContextInfo(serializableList);
            }
        }
        catch (Exception e)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, e.Message);
            return await fallback(cancellationToken).ConfigureAwait(false);
        }
    }
}
