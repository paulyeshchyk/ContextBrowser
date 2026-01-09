using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TTensor>
    where TTensor : notnull
{
    // context: ContextInfo, ContextInfoMatrix, build
    Task<IContextInfoDataset<ContextInfo, TTensor>> BuildAsync(IEnumerable<ContextInfo> contextsList, CancellationToken cancellation);
}
