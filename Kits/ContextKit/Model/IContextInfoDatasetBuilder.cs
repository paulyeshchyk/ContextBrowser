using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder<TKey>
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo, TKey> Build(IEnumerable<ContextInfo> contextsList);
}
