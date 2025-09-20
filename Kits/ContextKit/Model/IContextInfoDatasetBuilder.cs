using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TTensor>
    where TTensor : notnull
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo, TTensor> Build(IEnumerable<ContextInfo> contextsList);
}
