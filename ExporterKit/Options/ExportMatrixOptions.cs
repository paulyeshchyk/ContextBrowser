using ContextBrowserKit.Options;

namespace ExporterKit.Options;

public record ExportMatrixOptions(
    UnclassifiedPriority unclassifiedPriority,
    bool includeAllStandardActions)
{ }