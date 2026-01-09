using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    Task FillAsync(IContextInfoDataset<ContextInfo, TTensor> dataset, List<ContextInfo> elements, ExportMatrixOptions options, CancellationToken cancellationToken);
}
