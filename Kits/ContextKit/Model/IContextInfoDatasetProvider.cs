using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoDatasetProvider<TKey>
    where TKey : notnull
{
    Task<IContextInfoDataset<ContextInfo, TKey>> GetDatasetAsync(CancellationToken cancellationToken);
}
