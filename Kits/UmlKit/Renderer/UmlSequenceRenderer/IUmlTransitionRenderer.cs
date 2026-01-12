using System.Threading;
using System.Threading.Tasks;
using UmlKit.Builders.Model;
using UmlKit.DataProviders;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.DiagramGenerator.Renderer;

public interface IUmlTransitionRenderer<P>
    where P : IUmlParticipant
{
    Task<UmlDiagram<P>?> RenderAsync(GrouppedSortedTransitionList? allTransitions, CancellationToken cancellationToken);
}