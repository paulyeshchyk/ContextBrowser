using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model.CacheManager;

// context: relations, build
public interface IContextInfoCacheService
{
    // context: relations, build
    Task<IEnumerable<ContextInfo>> GetOrParseAndCacheAsync(
        Func<CancellationToken, Task<IEnumerable<ContextInfo>>> parseJob,
        Func<List<ContextInfoSerializableModel>, CancellationToken, Task<IEnumerable<ContextInfo>>> onRelationCallback,
        CancellationToken cancellationToken);
}
