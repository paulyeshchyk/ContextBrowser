using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders;

public class UmlRendererResult<TDiagram>
    where TDiagram : IUmlWritable
{
    public TDiagram? Diagram { get; init; }
    public UmlWriteOptions WriteOptions { get; init; }

    public UmlRendererResult(TDiagram? diagram, UmlWriteOptions writeOptions)
    {
        Diagram = diagram;
        WriteOptions = writeOptions;
    }
}
