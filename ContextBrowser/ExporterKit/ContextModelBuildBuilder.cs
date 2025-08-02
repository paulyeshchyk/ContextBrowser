using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowser.Infrastructure;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Parser.Content;

namespace ContextBrowser.ExporterKit;

// context: ContextInfo, build
public static class ContextModelBuildBuilder
{
    // context: ContextInfo, build
    public static ContextBuilderModel Build(AppOptions options, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "--- ContextModelBuildBuilder.Build ---");

        var contextsList = RoslynContextParser.Parse(options.theSourcePath, options.roslynCodeparserOptions, onWriteLog, cancellationToken);

        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, new ContextClassifier(), options.unclassifiedPriority, options.includeAllStandardActions);

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