using UmlKit.Model;

namespace UmlKit.Builders.TransitionFactory;

public class UmlTransitionParticipantFactory : IUmlTransitionFactory<UmlParticipant>
{
    public IUmlTransition<UmlParticipant> CreateTransition(IUmlParticipant from, IUmlParticipant to, string? label)
    {
        return new UmlTransitionParticipant((UmlParticipant)from, (UmlParticipant)to, new UmlArrow(), label);
    }

    public IUmlTransition<UmlParticipant> CreateTransition(IUmlParticipant from, IUmlParticipant to, UmlArrow arrow, string? label)
    {
        return new UmlTransitionParticipant((UmlParticipant)from, (UmlParticipant)to, arrow, label);
    }

    public IUmlParticipant CreateTransitionObject(string name)
    {
        return new UmlParticipant(name);
    }

    public IUmlParticipant CreateTransitionObject(string name, string alias)
    {
        return new UmlParticipant(name, alias);
    }
}
