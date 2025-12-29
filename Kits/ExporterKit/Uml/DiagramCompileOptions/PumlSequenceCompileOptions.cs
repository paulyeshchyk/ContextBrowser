using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public record PumlSequenceCompileOptions : IDiagramCompileOptions
{
    public string MetaItem { get; init; }

    public FetchType FetchType { get; init; }

    public string DiagramId { get; init; }

    public string DiagramTitle { get; init; }

    public string OutputFileName { get; init; }

    public PumlSequenceCompileOptions(string metaItem, FetchType fetchType, string diagramId, string diagramTitle, string outputFileName)
    {
        MetaItem = metaItem;
        FetchType = fetchType;
        DiagramId = diagramId;
        DiagramTitle = diagramTitle;
        OutputFileName = outputFileName;
    }
}
