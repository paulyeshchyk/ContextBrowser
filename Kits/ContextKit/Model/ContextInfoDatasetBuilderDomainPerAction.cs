using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;
using TensorKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilderDomainPerAction : IContextInfoDatasetBuilder<DomainPerActionTensor>
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IEnumerable<IContextInfoFiller<DomainPerActionTensor>> _contextInfoFiller;
    private readonly IAppOptionsStore _optionsStore;

    public ContextInfoDatasetBuilderDomainPerAction(IAppLogger<AppLevel> logger, IEnumerable<IContextInfoFiller<DomainPerActionTensor>> contextInfoFiller, IAppOptionsStore optionsStore)
    {
        _logger = logger;
        _contextInfoFiller = contextInfoFiller;
        _optionsStore = optionsStore;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset<ContextInfo, DomainPerActionTensor> Build(IEnumerable<ContextInfo> contextsList)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var exportOptions = _optionsStore.GetOptions<ExportMatrixOptions>();
        var classifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();

        var elements = contextsList.ToList();
        var dataset = new ContextInfoDatasetDomainPerAction<ContextInfo>();

        var orderedFillers = _contextInfoFiller.OrderBy(f => f.Order);
        foreach (var filler in orderedFillers)
        {
            filler.Fill(dataset, elements, exportOptions, classifier);
        }

        return dataset;
    }
}
