using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Matrix;
using LoggerKit;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public interface IContextInfoDatasetBuilder
{
    // context: ContextInfo, ContextInfoMatrix, build
    IContextInfoDataset Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier);
}

// context: ContextInfo, ContextInfoMatrix, build
public class ContextInfoDatasetBuilder : IContextInfoDatasetBuilder
{
    private readonly IAppLogger<AppLevel> _logger;

    public ContextInfoDatasetBuilder(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: ContextInfo, ContextInfoMatrix, build
    public IContextInfoDataset Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier)
    {
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var matrixBuilder = new ContextInfoDataBuilder(contextClassifier, matrixOptions);
        var matrix = matrixBuilder.BuildMatrix(contextsList.ToList());

        var contextLookup = ContextInfoElementTypeAndNameIndexBuilder.Build(contextsList.ToList());
        return new ContextInfoDataset(contextsList.ToList(), matrix, contextLookup);
    }
}