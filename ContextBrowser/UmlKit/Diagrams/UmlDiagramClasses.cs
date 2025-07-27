using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.UmlKit.Diagrams;

// context: uml, model
public class UmlDiagramClasses : UmlDiagram
{
    private readonly HashSet<UmlState> _states = new();
    private readonly List<UmlTransition> _transitions = new();

    // context: uml, create
    public override IUmlElement AddParticipant(string? name, UmlParticipantKeyword keyword = UmlParticipantKeyword.Participant)
    {
        var result = new UmlState(name);
        _states.Add(result);
        return result;
    }

    public override UmlDiagram AddParticipant(IUmlElement participant)
    {
        if(participant is UmlState state)
        {
            _states.Add(state);
            return this;
        }

        throw new ArgumentException($"UmlState is supported only {nameof(participant)}");
    }

    // context: uml, create
    public override IUmlElement AddTransition(string? from, string? to, string? label = null)
    {
        var theFrom = new UmlState(from);
        var theTo = new UmlState(to);
        return AddTransition(theFrom, theTo, label);
    }

    public override IUmlElement AddTransition(IUmlDeclarable from, IUmlDeclarable to, string? label = null)
    {
        if(from is not UmlState theFrom)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        if(to is not UmlState theTo)
        {
            throw new ArgumentException($"неподдерживаемый тип {nameof(from)}");
        }

        var result = new UmlTransition(theFrom, theTo, new UmlArrow(), label);
        _transitions.Add(result);
        return result;
    }

    // context: uml, share
    public override void WriteBody(TextWriter writer)
    {
        writer.WriteLine();

        foreach(var element in _elements)
        {
            element.WriteTo(writer);
        }
    }
}