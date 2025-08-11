using UmlKit.Model;

namespace UmlKit.Renderer.Builder;

public interface IUmlTransitionFactory<P>
    where P : IUmlParticipant
{
    IUmlTransition<P> CreateTransition(IUmlParticipant from, IUmlParticipant to, string? label = null);

    IUmlTransition<P> CreateTransition(IUmlParticipant from, IUmlParticipant to, UmlArrow arrow, string? label = null);

    IUmlParticipant CreateTransitionObject(string name);

    IUmlParticipant CreateTransitionObject(string name, string alias);
}
