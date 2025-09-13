using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider
{
    Task<IContextKeyMap<ContextInfo, IContextKey>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
