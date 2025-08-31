using UmlKit.Builders;

namespace ExporterKit.DiagramCompiler.DiagramCompilerOptions;

public interface IDiagramCompileOptions
{
    string MetaItem { get; init; }
    FetchType FetchType { get; init; }
    string DiagramTitle { get; init; }
    string DiagramId { get; init; }
    string OutputFileName { get; init; }
}
