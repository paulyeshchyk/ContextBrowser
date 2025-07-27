using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: model, uml
// pattern: Template method
// pattern note: subclassing
public class UmlDiagramSequence : UmlDiagram
{
    private readonly HashSet<UmlParticipant> _participants = new();
    private readonly List<UmlSequence> _transitions = new();

    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlParticipant(name, keyword);
        _participants.Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlParticipant theParticipant)
        {
            _participants.Add(theParticipant);
            return this;
        }

        throw new ArgumentException($"UmlParticipant is supported only {nameof(participant)}");
    }

    // context: uml, create
    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        var theFrom = new UmlParticipant(from);
        var theTo = new UmlParticipant(to);

        return AddTransition(theFrom, theTo, label);
    }

    // context: uml, create
    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        if(from is not UmlParticipant theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if(to is not UmlParticipant theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }
        var result = new UmlSequence(theFrom, theTo, label);
        _transitions.Add(result);
        return result;
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
