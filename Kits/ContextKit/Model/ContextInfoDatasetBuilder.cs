using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using LoggerKit;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilder : IContextInfoDatasetBuilder
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IEnumerable<IContextInfoFiller> _contextInfoFiller;

    public ContextInfoDatasetBuilder(IAppLogger<AppLevel> logger, IEnumerable<IContextInfoFiller> contextInfoFiller)
    {
        _logger = logger;
        _contextInfoFiller = contextInfoFiller;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset<ContextInfo> Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier)
    {
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var elements = contextsList.ToList();
        var dataset = new ContextInfoDataset<ContextInfo>();

        var orderedFillers = _contextInfoFiller.OrderBy(f => f.Order);
        foreach (var filler in orderedFillers)
        {
            filler.Fill(dataset, elements, matrixOptions, contextClassifier);
        }

        return dataset;
    }
}
