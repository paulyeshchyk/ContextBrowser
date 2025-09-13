using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoIndexerProvider
{
    Task<IContextKeyIndexer<ContextInfo>> GetIndexerAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
