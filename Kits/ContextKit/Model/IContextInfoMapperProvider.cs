using System;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider
{
    Task<DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
