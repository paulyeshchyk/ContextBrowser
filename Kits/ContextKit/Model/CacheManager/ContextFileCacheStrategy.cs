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
using ContextKit.Model.Factory;
using LoggerKit;

namespace ContextKit.Model.CacheManager;

public class ContextFileCacheStrategy : IFileCacheStrategy
{
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public ContextFileCacheStrategy(IAppLogger<AppLevel> appLogger)
    {
        _appLogger = appLogger;
    }

    public async Task<IEnumerable<ContextInfo>> ReadAsync(CacheJsonModel cacheModel, Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback, CancellationToken cancellationToken)
    {
        if (await ShouldRebuildAsync(cacheModel, cancellationToken))
        {
            return [];
        }

        try
        {
            var fileContent = await File.ReadAllTextAsync(cacheModel.Input, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(fileContent))
                return [];

            var serializableList = JsonSerializer.Deserialize<List<ContextInfoSerializableModel>>(fileContent, _jsonOptions);
            if (serializableList == null)
                return [];

            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Returning data from file cache.");
            return await onRelationCallback(serializableList, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, $"Cache read error: {ex.Message}");
            return [];
        }
    }

    public async Task SaveAsync(IEnumerable<ContextInfo> contexts, CacheJsonModel cacheModel, CancellationToken cancellationToken)
    {
        // Использование атомарного сохранения
        var tempFilePath = Path.GetTempFileName();
        try
        {
            var serializableList = ContextInfoSerializableModelAdapter.Adapt(contexts.ToList());
            var json = JsonSerializer.Serialize(serializableList, _jsonOptions);
            await File.WriteAllTextAsync(tempFilePath, json, cancellationToken).ConfigureAwait(false);

            if (File.Exists(cacheModel.Output))
                File.Delete(cacheModel.Output);

            var directoryPath = (new FileInfo(cacheModel.Output)).Directory?.FullName;
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new Exception($"Wrong directory path for file: {cacheModel.Output}");
            }
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            File.Move(tempFilePath, cacheModel.Output);
        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Cache save error: {ex.Message}");
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }

    public Task<bool> ShouldRebuildAsync(CacheJsonModel cacheModel, CancellationToken cancellationToken)
    {
        var result = !File.Exists(cacheModel.Input) || cacheModel.RenewCache;
        return Task.FromResult(result);
    }
}
