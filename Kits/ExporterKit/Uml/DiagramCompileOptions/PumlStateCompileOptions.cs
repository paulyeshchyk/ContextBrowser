using UmlKit.Builders;

namespace ExporterKit.Uml.DiagramCompileOptions;

public record PumlStateCompileOptions : IDiagramCompileOptions
{
    public string MetaItem { get; init; }

    public FetchType FetchType { get; init; }

    public string DiagramTitle { get; init; }

    public string DiagramId { get; init; }

    public string OutputFileName { get; init; }

    public PumlStateCompileOptions(string metaItem, FetchType fetchType, string diagramTitle, string diagramId, string outputFileName)
    {
        MetaItem = metaItem;
        FetchType = fetchType;
        DiagramTitle = diagramTitle;
        DiagramId = diagramId;
        OutputFileName = outputFileName;
    }
}