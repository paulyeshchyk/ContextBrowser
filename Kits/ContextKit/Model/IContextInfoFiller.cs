using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextBrowserKit.Options.Export;

namespace ContextKit.Model;

/// <summary>
/// Интерфейс для всех "заполнителей" (fillers).
/// </summary>

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoFiller<TKey>
    where TKey : notnull
{
    int Order { get; }

    // context: ContextInfo, ContextInfoMatrix, build
    void Fill(
        IContextInfoDataset<ContextInfo, TKey> dataset,
        List<ContextInfo> elements,
        ExportMatrixOptions options,
        IDomainPerActionContextTensorClassifier classifier);
}
