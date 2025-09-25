using System.Collections.Generic;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

/// <summary>
/// Интерфейс для всех "заполнителей" (fillers).
/// </summary>
// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoFiller<TTensor>
    where TTensor : notnull
{
    int Order { get; }

    // context: ContextInfo, ContextInfoMatrix, build
    void Fill(IContextInfoDataset<ContextInfo, TTensor> dataset, List<ContextInfo> elements, ExportMatrixOptions options);
}
