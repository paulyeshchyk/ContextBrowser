using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

public interface IContextInfoDatasetProvider<TTensor>
    where TTensor : notnull
{
    Task<IContextInfoDataset<ContextInfo, TTensor>> GetDatasetAsync(CancellationToken cancellationToken);
}
