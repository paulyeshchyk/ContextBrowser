using System;
using System.Threading;
using System.Threading.Tasks;
using TensorKit.Model;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider<TTensor>
    where TTensor : notnull
{
    Task<IContextInfo2DMap<ContextInfo, TTensor>> GetMapperAsync(MapperKeyBase mapperType, CancellationToken cancellationToken);
}
