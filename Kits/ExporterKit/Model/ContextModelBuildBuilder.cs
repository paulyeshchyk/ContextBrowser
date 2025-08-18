using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Model;
using RoslynKit.Model;
using RoslynKit.Phases;

namespace ExporterKit.Uml;

// context: ContextInfo, build
public static class ContextModelBuildBuilder
{
    // context: ContextInfo, build
    public static ContextBuilderModel Build(RoslynOptions roslynOptions, ExportMatrixOptions matrixOptions, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "--- ContextModelBuildBuilder.Build ---");

        var contextsList = RoslynContextParser.Parse(roslynOptions, contextClassifier, onWriteLog, cancellationToken);

        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, contextClassifier, matrixOptions);

        var contextLookup = GenerateContextLookup(contextsList);
        return new ContextBuilderModel(contextsList, matrix, contextLookup);
    }

    private static Dictionary<string, ContextInfo> GenerateContextLookup(List<ContextInfo> contextsList)
    {
        return contextsList
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner?.Name)
                ? $"{c.ClassOwner?.Name}.{c.Name}"
                : $"{c.Name}")
            .ToDictionary(g => g.Key, g => g.First());
    }
}