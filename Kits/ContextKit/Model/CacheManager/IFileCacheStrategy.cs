using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ContextBrowser.FileManager;

// Новый интерфейс для работы с файлами
public interface IFileCacheStrategy
{
    Task<IEnumerable<ContextInfo>> ReadAsync(CacheJsonModel cacheModel, Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback, CancellationToken cancellationToken);

    Task SaveAsync(IEnumerable<ContextInfo> contexts, CacheJsonModel cacheModel, CancellationToken cancellationToken);

    Task<bool> ShouldRebuildAsync(CacheJsonModel cacheModel, CancellationToken cancellationToken);
}
