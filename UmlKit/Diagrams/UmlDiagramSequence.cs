using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram
{
    private readonly HashSet<UmlParticipant> _participants = new();
    private readonly List<UmlSequence> _transitions = new();

    public override UmlDiagram AddParticipant(string name)
    {
        _participants.Add(new UmlParticipant(name));
        return this;
    }

    // context: uml, create
    public override UmlDiagram AddTransition(string from, string to, string? label = null)
    {
        _transitions.Add(new UmlSequence(new UmlParticipant(from), new UmlParticipant(to), label));
        return this;
    }

    public override void WriteBody(TextWriter writer)
    {
        foreach(var participant in _participants.Distinct())
            participant.WriteTo(writer);

        writer.WriteLine();

        foreach(var transition in _transitions)
        {
            transition.WriteTo(writer);
        }
    }
}
