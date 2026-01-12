using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders;

public interface IUmlTransitionFactory<P>
    where P : IUmlParticipant
{
    IUmlTransition<P> CreateTransition(P from, P to, string? label = null);

    IUmlTransition<P> CreateTransition(P from, P to, UmlArrow arrow, string? label = null);

    P CreateTransitionObject(string name);

    P CreateTransitionObject(string name, string alias);
}
