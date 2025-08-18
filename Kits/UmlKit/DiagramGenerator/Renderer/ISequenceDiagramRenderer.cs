using UmlKit.Builders.Model;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public interface ISequenceDiagramRenderer<P>
    where P : IUmlParticipant
{
    void Render(UmlDiagram<P> diagram, GrouppedSortedTransitionList? allTransitions);
}