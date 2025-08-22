using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Matrix;
using ExporterKit.Model;

namespace ExporterKit.Uml;

// context: ContextInfo, ContextInfoMatrix, build
public static class ContextInfoMatrixModelBuilder
{
    // context: ContextInfo, ContextInfoMatrix, build
    public static ContextInfoMatrixModel Build(IEnumerable<ContextInfo> contextsList, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier, OnWriteLog? onWriteLog)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "--- ContextMatrixBuilder.Build ---");

        var matrixBuilder = new ContextMatrixBuilder(contextClassifier, matrixOptions);
        var matrix = matrixBuilder.BuildMatrix(contextsList.ToList());

        var contextLookup = ContextInfoElementTypeAndNameIndexBuilder.Build(contextsList.ToList());
        return new ContextInfoMatrixModel(contextsList.ToList(), matrix, contextLookup);
    }
}