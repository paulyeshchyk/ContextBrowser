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

// Новая реализация, которая инкапсулирует всю файловую логику
public class ContextFileCacheStrategy : IFileCacheStrategy
{
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IContextInfoRelationManager _relationManager;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public ContextFileCacheStrategy(IAppLogger<AppLevel> appLogger, IContextInfoRelationManager relationManager)
    {
        _appLogger = appLogger;
        _relationManager = relationManager;
    }

    public async Task<IEnumerable<ContextInfo>> ReadAsync(CacheJsonModel cacheModel, Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback, CancellationToken cancellationToken)
    {
        if (await ShouldRebuildAsync(cacheModel, cancellationToken))
        {
            return Enumerable.Empty<ContextInfo>();
        }

        try
        {
            var fileContent = await File.ReadAllTextAsync(cacheModel.Input, cancellationToken);
            if (string.IsNullOrEmpty(fileContent))
                return Enumerable.Empty<ContextInfo>();

            var serializableList = JsonSerializer.Deserialize<List<ContextInfoSerializableModel>>(fileContent, _jsonOptions);
            if (serializableList == null)
                return Enumerable.Empty<ContextInfo>();

            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Returning data from file cache.");
            return await onRelationCallback(serializableList, cancellationToken);
        }
        catch (Exception ex)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Exception, $"Cache read error: {ex.Message}");
            return Enumerable.Empty<ContextInfo>();
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
            await File.WriteAllTextAsync(tempFilePath, json, cancellationToken);

            if (File.Exists(cacheModel.Output))
                File.Delete(cacheModel.Output);

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
