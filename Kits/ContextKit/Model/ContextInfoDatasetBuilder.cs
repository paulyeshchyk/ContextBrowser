using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using LoggerKit;

namespace ContextKit.Model;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilder<TTensor> : IContextInfoDatasetBuilder<TTensor>
    where TTensor : notnull
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IEnumerable<IContextInfoFiller<TTensor>> _contextInfoFiller;
    private readonly IAppOptionsStore _optionsStore;

    public ContextInfoDatasetBuilder(IAppLogger<AppLevel> logger, IEnumerable<IContextInfoFiller<TTensor>> contextInfoFiller, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _contextInfoFiller = contextInfoFiller;
        _optionsStore = optionsStore;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset<ContextInfo, TTensor> Build(IEnumerable<ContextInfo> contextsList, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        cancellationToken.ThrowIfCancellationRequested();

        var exportOptions = _optionsStore.GetOptions<ExportMatrixOptions>();

        var elements = contextsList.ToList();
        var dataset = new ContextInfoDataset<ContextInfo, TTensor>();

        var orderedFillers = _contextInfoFiller.OrderBy(f => f.Order);
        foreach (var filler in orderedFillers)
        {
            filler.Fill(dataset, elements, exportOptions);
        }

        return dataset;
    }
}
