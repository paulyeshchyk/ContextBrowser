using System;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider<TKey>
    where TKey : notnull
{
    Task<IContextInfo2DMap<ContextInfo, TKey>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
