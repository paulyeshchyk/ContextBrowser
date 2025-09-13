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

/// <summary>
/// Управляет сохранением и чтением списка объектов ContextInfo
/// </summary>
// context: relations, build
public class ContextInfoCacheService : IContextInfoCacheService
{
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IFileCacheStrategy _cacheStrategy;

    private Task<IEnumerable<ContextInfo>>? _inMemoryCacheTask;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ContextInfoCacheService(IFileCacheStrategy fileCacheStrategy, IAppLogger<AppLevel> appLogger)
    {
        _appLogger = appLogger;
        _cacheStrategy = fileCacheStrategy;
    }

    public async Task<IEnumerable<ContextInfo>> GetOrParseAndCacheAsync(
        CacheJsonModel cacheModel,
        Func<CancellationToken, Task<IEnumerable<ContextInfo>>> parseJob,
        Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback,
        CancellationToken cancellationToken)
    {
        if (_inMemoryCacheTask?.Status == TaskStatus.RanToCompletion)
        {
            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, "Returning data from in-memory cache.");
            return _inMemoryCacheTask.Result;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_inMemoryCacheTask?.Status == TaskStatus.RanToCompletion)
            {
                return _inMemoryCacheTask.Result;
            }

            var contextsFromFile = await _cacheStrategy.ReadAsync(cacheModel, onRelationCallback, cancellationToken);
            if (contextsFromFile.Any())
            {
                _inMemoryCacheTask = Task.FromResult(contextsFromFile);
                return contextsFromFile;
            }

            _appLogger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "Cache not found. Starting parsing job.");

            _inMemoryCacheTask = parseJob(cancellationToken);
            var parsingResult = await _inMemoryCacheTask;

            SaveOnBackground(parsingResult, cacheModel, cancellationToken);

            return parsingResult;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void SaveOnBackground(IEnumerable<ContextInfo> parsingResult, CacheJsonModel cacheModel, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            await _cacheStrategy.SaveAsync(parsingResult, cacheModel, cancellationToken).ConfigureAwait(false);
        }, cancellationToken);
    }
}
