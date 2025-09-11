using System.Collections.Generic;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset<ContextInfo> Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier);
}
