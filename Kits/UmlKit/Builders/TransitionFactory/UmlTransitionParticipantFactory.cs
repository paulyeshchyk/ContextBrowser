using UmlKit.PlantUmlSpecification;

namespace UmlKit.Builders.TransitionFactory;

public class UmlTransitionParticipantFactory<P> : IUmlTransitionFactory<P>
where P : IUmlParticipant
{
    public IUmlTransition<P> CreateTransition(P from, P to, string? label)
    {
        return new UmlTransitionParticipant<P>(from, to, new UmlArrow(), label);
    }

    public IUmlTransition<P> CreateTransition(P from, P to, UmlArrow arrow, string? label)
    {
        return new UmlTransitionParticipant<P>(from, to, arrow, label);
    }

    public P CreateTransitionObject(string name)
    {
        IUmlParticipant p = new UmlParticipant(name);
        return (P)p;
    }

    public P CreateTransitionObject(string name, string alias)
    {
        IUmlParticipant p = new UmlParticipant(name, alias);
        return (P)p;
    }
}
