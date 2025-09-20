using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TKey>
    where TKey : notnull
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo, TKey> Build(IEnumerable<ContextInfo> contextsList);
}
