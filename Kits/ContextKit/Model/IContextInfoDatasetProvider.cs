using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoDatasetProvider
{
    Task<IContextInfoDataset<ContextInfo>> GetDatasetAsync(CancellationToken cancellationToken);
}
