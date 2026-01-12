using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders.TransitionFactory;

public class UmlTransitionStateFactory<P> : IUmlTransitionFactory<P>
    where P : IUmlParticipant
{
    public IUmlTransition<P> CreateTransition(P from, P to, string? label)
    {
        return new UmlTransitionState<P>(from, to, new UmlArrow(), label);
    }

    public IUmlTransition<P> CreateTransition(P from, P to, UmlArrow arrow, string? label)
    {
        return new UmlTransitionState<P>(from, to, arrow, label);
    }

    public P CreateTransitionObject(string name)
    {
        IUmlParticipant p = new UmlState(name, null);
        return (P)p;
    }

    public P CreateTransitionObject(string name, string? alias)
    {
        IUmlParticipant p = new UmlState(name, alias);
        return (P)p;
    }
}
