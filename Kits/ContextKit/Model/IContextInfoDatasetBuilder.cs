using System.Collections.Generic;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TTensor>
    where TTensor : notnull
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo, TTensor> Build(IEnumerable<ContextInfo> contextsList);
}
