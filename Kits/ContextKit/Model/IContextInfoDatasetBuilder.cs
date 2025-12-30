using System.Collections.Generic;
using System.Threading;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TTensor>
    where TTensor : notnull
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo, TTensor> Build(IEnumerable<ContextInfo> contextsList, CancellationToken cancellation);
}
