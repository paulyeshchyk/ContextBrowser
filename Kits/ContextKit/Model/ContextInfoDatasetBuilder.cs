using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Matrix;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public static class ContextInfoDatasetBuilder
{
    // context: ContextInfo, ContextInfoMatrix, build
    public static IContextInfoDataset Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier, OnWriteLog? onWriteLog)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var matrixBuilder = new ContextInfoDataBuilder(contextClassifier, matrixOptions);
        var matrix = matrixBuilder.BuildMatrix(contextsList.ToList());

        var contextLookup = ContextInfoElementTypeAndNameIndexBuilder.Build(contextsList.ToList());
        return new ContextInfoDataset(contextsList.ToList(), matrix, contextLookup);
    }
}