using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoIndexerProvider
{
    Task<IKeyIndexBuilder<ContextInfo>> GetIndexerAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
