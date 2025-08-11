using UmlKit.Model;
using UmlKit.Renderer.Builder;

namespace UmlKit.Renderer.TransitionFactory;

public class UmlTransitionStateFactory : IUmlTransitionFactory<UmlState>
{
    public IUmlTransition<UmlState> CreateTransition(IUmlParticipant from, IUmlParticipant to, string? label)
    {
        return new UmlTransitionState((UmlState)from, (UmlState)to, new UmlArrow(), label);
    }

    public IUmlTransition<UmlState> CreateTransition(IUmlParticipant from, IUmlParticipant to, UmlArrow arrow, string? label)
    {
        return new UmlTransitionState((UmlState)from, (UmlState)to, arrow, label);
    }


    public IUmlParticipant CreateTransitionObject(string name)
    {
        return new UmlState(name, null);
    }

    public IUmlParticipant CreateTransitionObject(string name, string? alias)
    {
        return new UmlState(name, alias);
    }
}