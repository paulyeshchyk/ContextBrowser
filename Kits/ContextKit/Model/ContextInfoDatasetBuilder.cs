using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ContextKit.Model.Collector;
using LoggerKit;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilder<TKey> : IContextInfoDatasetBuilder<TKey>
    where TKey : notnull
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IEnumerable<IContextInfoFiller<TKey>> _contextInfoFiller;
    private readonly IAppOptionsStore _optionsStore;

    public ContextInfoDatasetBuilder(IAppLogger<AppLevel> logger, IEnumerable<IContextInfoFiller<TKey>> contextInfoFiller, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _contextInfoFiller = contextInfoFiller;
        _optionsStore = optionsStore;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset<ContextInfo, TKey> Build(IEnumerable<ContextInfo> contextsList)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var exportOptions = _optionsStore.GetOptions<ExportMatrixOptions>();

        var elements = contextsList.ToList();
        var dataset = new ContextInfoDataset<ContextInfo, TKey>();

        var orderedFillers = _contextInfoFiller.OrderBy(f => f.Order);
        foreach (var filler in orderedFillers)
        {
            filler.Fill(dataset, elements, exportOptions);
        }

        return dataset;
    }
}
