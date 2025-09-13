using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ContextBrowser.FileManager;

// context: relations, build
public interface IContextInfoCacheService
{
    // context: relations, build
    Task<IEnumerable<ContextInfo>> GetOrParseAndCacheAsync(
        CacheJsonModel cacheModel,
        Func<CancellationToken, Task<IEnumerable<ContextInfo>>> parseJob,
        Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback,
        CancellationToken cancellationToken);
}
