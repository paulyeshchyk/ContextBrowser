using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoMapperProvider
{
    Task<IContextKeyMap<ContextInfo>> GetMapperAsync(MapperType mapperType, CancellationToken cancellationToken);
}

public enum MapperType
{
    DomainPerAction
}